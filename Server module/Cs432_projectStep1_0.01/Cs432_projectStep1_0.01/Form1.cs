using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Cs432_projectStep1_0._01
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool listening = false;
        bool remoteConncted = false;
        byte[] Aes128Key;
        byte[] IV;
        string RSAkeyPair = "";
        string SignKeyPair = "";
        string databaseValues = "";

        class Client
        {
            public Socket Socket;
            public string Name;
            public byte[] authenticationKey ;
            public byte[] aesKey ;
        }

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        List<Client> socketList = new List<Client>();
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
            ServerConsol.AppendText("Please enter the password to start server");
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void button_startServer_Click(object sender, EventArgs e)
        {
            string EncDecPubText, SignKeyText;

            //byte[] deneme = hashWithSHA256("123");
            //byte[] parolayarım = new byte[16];
            //Array.Copy(deneme, 0, parolayarım, 0, 16);
            //string parolayarıHex = generateHexStringFromByteArray(parolayarım);

            string password = "Bohemian";
            byte[] sha256 = hashWithSHA256(password);

            string key128 = "0123456789ABCDEF";
            byte[] byteKey128 = Encoding.Default.GetBytes(key128);
            Array.Copy(sha256, 16, byteKey128, 0, 16); // Determine the Key from least significant bit of hashed password

            string IV = "0123456789ABCDEF";
            byte[] byteIV = Encoding.Default.GetBytes(IV);
            Array.Copy(sha256, 0, byteIV, 0, 16); // Determine the IV from most significant bit of hashed password



            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(@"C:\encrypted_server_enc_dec_pub_prv.txt"))
            {
                EncDecPubText = Encoding.Default.GetString(hexStringToByteArray(fileReader.ReadLine()));
                // Do decryption operation here into decryptedByteArray
                byte[] decryptedAES128 = decryptWithAES128(EncDecPubText, byteKey128, byteIV);
                RSAkeyPair = Encoding.Default.GetString(decryptedAES128);
            }



            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(@"C:\encrypted_server_signing_verification_pub_prv.txt"))
            {
                SignKeyText = Encoding.Default.GetString(hexStringToByteArray(fileReader.ReadLine()));
                // Do decryption operation here into decryptedByteArray
                byte[] decryptedAES128 = decryptWithAES128(SignKeyText, byteKey128, byteIV);
                SignKeyPair = Encoding.Default.GetString(decryptedAES128);
            }




            int serverPort;
            Thread acceptThread;

            if (Int32.TryParse(textBox_Port.Text, out serverPort))
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                serverSocket.Listen(3);

                listening = true;
                button_startServer.Enabled = false;
                acceptThread = new Thread(new ThreadStart(Accept));
                acceptThread.Start();

                ServerConsol.AppendText("\nStarted listening on port: " + serverPort + "\n");
            }
            else
            {
                ServerConsol.AppendText("Please check port number \n");
            }
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {

                    Client newClient = new Client { Socket = serverSocket.Accept()};

                    string loginOrEnroll = "";
                    try
                    {
                        loginOrEnroll = receiveMessage(newClient.Socket); // client enroll mu olmak istiyor login 
                    }
                    catch
                    {
                        ServerConsol.AppendText("an error occured while receiving message ERROR_1_\n");
                    }
                    byte[] loginOrEnrollBytes = decryptWithRSA(loginOrEnroll, 3072, RSAkeyPair);
                    loginOrEnroll = Encoding.Default.GetString(loginOrEnrollBytes);
                    string databasePath = @"C:\database.txt";

                    if (loginOrEnroll == "enroll") // if the request is enroll 
                        Enroll(newClient, databasePath); 
                    else if (loginOrEnroll == "login") // if the request is login request 
                        Login(newClient, databasePath);  
                }

                catch
                {
                    if (terminating)
                        listening = false;
                    else
                        ServerConsol.AppendText("The socket stopped working \n");
                }
            }
        }

        private void ReceiveFunc(Client c, byte[] aesEncDecKey, byte[] authentKey) // SESSION KEY PARAMETRE OLARAK ALINMALI ??
        {
            Socket s = c.Socket;
            bool myThreadTerminating = false;
            while(!myThreadTerminating)
            {
                try
                {
                    string myMessage = receiveMessage(s);
                    // After Received message from Client, Server decrypte it then try to verify it with HMAC
                    byte[] byteIV = new byte[16];
                    using (var rng = new RNGCryptoServiceProvider()) // Generate 128Bit random number for IV
                    {
                        rng.GetBytes(byteIV);
                    }
                    byte[] decryptedAES128 = decryptWithAES128(myMessage, aesEncDecKey, byteIV);

                    // Broadcast Message to the other clients,  
                    foreach (Client client in socketList )
                    {
                        if (client != c) // Mesaj kriptik bir şekilde gitmeli
                        {
                            sendMessage(myMessage, client.Socket);
                        }
                    }
                }
                catch
                {
                    ServerConsol.AppendText("A problem occured while receiving message\n");
                    myThreadTerminating = true;
                }
                
            }
        }


        /*
         * Helper function for receiving and sending half-dynamically created arrays
         * */

        private void sendMessageWithSign(string m, Socket s)
        {

        }

      

        private void sendMessage(byte[] b, Socket s)
        {
            Thread.Sleep(1000);

            byte[] a = new byte[10000];
            a = Encoding.Default.GetBytes(b.Length.ToString());
            s.Send(a);

            s.Send(b);
        }

        private void sendMessage(string m, Socket s)
        {
            Thread.Sleep(1000);

            Byte[] a = new Byte[10000];
            a = Encoding.Default.GetBytes(m.Length.ToString());
            s.Send(a);

            a = new Byte[m.Length];
            a = Encoding.Default.GetBytes(m);
            s.Send(a);
        }

        private string receiveMessage(Socket s)
        {
            string m;
            int byteLength;
            Byte[] a = new Byte[100];
            s.Receive(a);
            Int32.TryParse(Encoding.Default.GetString(a), out byteLength);

            a = new Byte[byteLength];
            s.Receive(a);
            m = Encoding.Default.GetString(a);

            return m;
        }

        private void Enroll(Client newClient, string databasePath)
        {
            // LOGIN TUSU INAKTIF EDİLSİN
            ServerConsol.AppendText("A client tries to Enroll to the server\n");
            string usernamePass = receiveMessage(newClient.Socket);

            // decryption with RSA 3072
            byte[] usernamePW = decryptWithRSA(usernamePass, 3072, RSAkeyPair); // Decyrpte password and username

            byte[] passwordHalfByte = new Byte[16];
            Array.Copy(usernamePW, 0, passwordHalfByte, 0, 16); // Parse half of the password

            byte[] usernameByte = new Byte[usernamePW.Length - 16];
            Array.Copy(usernamePW, 16, usernameByte, 0, ((usernamePW.Length - 16))); // Parse the rest of it, to find username

            string usernameHex = Encoding.Default.GetString(usernameByte); // convert byte to string for username 
            string passwordHalfHex = Encoding.Default.GetString(passwordHalfByte); // convert byte to string for password


            string line = "";
            bool trueMatch = false, usernameFound = false;



            try
            {
                if (System.IO.File.Exists(databasePath)) // If txt file exists
                {
                    ServerConsol.AppendText("Database file found\n");
                    using (System.IO.StreamReader sr = System.IO.File.OpenText(databasePath)) // read file  
                    {
                        databaseValues = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            databaseValues += line + "\n";
                            if (line == usernameHex) // if username is found in database
                            {
                                ServerConsol.AppendText("Username already exist\n");
                                usernameFound = true;

                                byte[] passwordErrorSignedByte = signWithRSA("Error!Username Exists", 3072, SignKeyPair);
                                byte[] message = Encoding.Default.GetBytes("Error!Username Exists");

                                byte[] concatenatedByte = new byte[384 + message.Length];

                                Array.Copy(passwordErrorSignedByte, 0, concatenatedByte, 0, 384);
                                Array.Copy(message, 0, concatenatedByte, 384, message.Length);

                                sendMessage(concatenatedByte, newClient.Socket); // send signed error message
                                newClient.Socket.Close(); 
                                break;
                            }
                        }

                        sr.Close();
                        if (usernameFound == false) // if username couldn't be founded at database , add user to the database aka Enrollment 
                        {

                            newClient.Name = usernameHex; // After connection the username of the client added to connected client List
                            socketList.Add(newClient);

                            ServerConsol.AppendText("Saving user to the database\n");
                            StreamWriter sw = new StreamWriter(databasePath);
                            databaseValues += usernameHex + "\n" + passwordHalfHex + "\n";
                            sw.Write(databaseValues);
                            ServerConsol.AppendText("A user successfully enrolled to the database\n");
                            sw.Close();
                            byte[] passwordSuccessSignedByte = signWithRSA("Succesfully Enrolled", 3072, SignKeyPair);
                            byte[] message = Encoding.Default.GetBytes("Succesfully Enrolled");

                            byte[] concatenatedByte = new byte[384 + message.Length];

                            Array.Copy(passwordSuccessSignedByte, 0, concatenatedByte, 0, 384);
                            Array.Copy(message, 0, concatenatedByte, 384, message.Length);


                            sendMessage(concatenatedByte, newClient.Socket); // send signed success message
                            newClient.Socket.Close();

                        }
                    }
                }
                else // if txt file doesn't exist 
                {
                    ServerConsol.AppendText("database file created\n");
                    File.Create(databasePath); // create new one

                    using (System.IO.TextWriter sw = new System.IO.StreamWriter(databasePath))
                    {
                        sw.WriteLine(usernameHex);
                        sw.WriteLine(passwordHalfHex);
                    }

                    byte[] passwordSuccessSignedByte = signWithRSA("Succesfully Enrolled", 3072, SignKeyPair);
                    byte[] message = Encoding.Default.GetBytes("Succesfully Enrolled");

                    byte[] concatenatedByte = new byte[384 + message.Length];

                    Array.Copy(passwordSuccessSignedByte, 0, concatenatedByte, 0, 384);
                    Array.Copy(message, 0, concatenatedByte, 384, message.Length);

                    //socketList.Add(newClient);
                    sendMessage(concatenatedByte, newClient.Socket); // send signed success message 
                    newClient.Socket.Close();

                    //Thread receiveThread = new Thread(
                    //    o =>
                    //    {
                    //        ReceiveFunc((Client)o);
                    //    }
                    //);
                    //receiveThread.Start(newClient);
                }
            }
            catch
            {
                ServerConsol.AppendText("A problem occured while a client tries to Enroll\n");
            }
        }

        private void Login(Client newClient, string databasePath)
        {
            ServerConsol.AppendText("A client tries to login to the server\n");
            string authenticationRequest = "";
            try
            {
                authenticationRequest = receiveMessage(newClient.Socket);                  //Authentication request message    RECEIVE
            }
            catch
            {
                ServerConsol.AppendText("A problem occured while receiving message ERROR_2_\n");
            }

            byte[] authenticationRequestByte = decryptWithRSA(authenticationRequest, 3072, RSAkeyPair); // Decyrpte username and it's hash value

            byte[] hashOfUsernameByte = new Byte[32];
            Array.Copy(authenticationRequestByte, 0, hashOfUsernameByte, 0, 32); // Get first 16 byte to find the hash of the username

            byte[] usernameByte = new Byte[authenticationRequestByte.Length - 32];
            Array.Copy(authenticationRequestByte, 32, usernameByte, 0, ((authenticationRequestByte.Length - 32))); // Parse the rest of it, to find username

            string username = Encoding.Default.GetString(usernameByte); // convert byte to string for username 
            bool usernameFound = false;

            byte[] usernameHashdeneme = hashWithSHA256(username);

            if (Encoding.Default.GetString(usernameHashdeneme) == Encoding.Default.GetString(hashOfUsernameByte)) // eğer yollanan kullanıcı adıyla hash value su birbirlerini tutuyorsa
            {
                // Generate 128Bit random number
                byte[] random128BitNumber = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(random128BitNumber);
                }
                //////////////////
                byte[] hashOfRandumNum = hashWithSHA256(Encoding.Default.GetString(random128BitNumber));
                byte[] concatenatedByte = new byte[48];

                Array.Copy(hashOfRandumNum, 0, concatenatedByte, 0, 32);
                Array.Copy(random128BitNumber, 0, concatenatedByte, 32, 16);

                try
                {
                    sendMessage(concatenatedByte, newClient.Socket); // send random number to the client               SEND RANDOM NUMBER SEEEEND
                }
                catch
                {
                    ServerConsol.AppendText("A problem occured while sending message ERROR_3_\n");
                }

                string HMACclient = receiveMessage(newClient.Socket); // receive HMAC value from client                //RECEIIIVEEEE


                if (System.IO.File.Exists(databasePath)) // If txt file exists
                {
                    using (System.IO.StreamReader sr = System.IO.File.OpenText(databasePath)) // read file  
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line == username) // if username is found in database ***TRY TO VERIFY HMAC***
                            {
                                usernameFound = true;
                                string pass = sr.ReadLine();
                                byte[] HMACserver = applyHMACwithSHA256(Encoding.Default.GetString(random128BitNumber), Encoding.Default.GetBytes(pass));
                                // IF SERVER CAN VERIFY THE HMAC OF THE CLIENT
                                if (Encoding.Default.GetString(HMACserver) == HMACclient)
                                {
                                    bool allreadyInServer = false;
                                    foreach (Client client in socketList)
                                    {
                                        if (client.Name == username) // Verify edildikten sonra Client aynı username ile halihazırda Server a bağlı mı ?? 
                                        {
                                            allreadyInServer = true;
                                            //send signed succesfully verified message to the client
                                            byte[] passwordSuccessSignedByte = signWithRSA("Succesfully Verified But You are Allready In Server", 3072, SignKeyPair); // sign message
                                            byte[] message = Encoding.Default.GetBytes("Succesfully Verified But You are Allready In Server");

                                            byte[] HMACacknowledgementByte = new byte[384 + message.Length];

                                            Array.Copy(passwordSuccessSignedByte, 0, HMACacknowledgementByte, 0, 384);
                                            Array.Copy(message, 0, HMACacknowledgementByte, 384, message.Length);

                                            sendMessage(HMACacknowledgementByte, newClient.Socket); // send message
                                            ServerConsol.AppendText("Username found But User is Allready In Server\n");
                                            newClient.Socket.Close();
                                            break;
                                            // Verify edildi ama serverda sın zaten
                                        }
                                    }
                                    if (allreadyInServer == false)
                                    {
                                        //send signed succesfully verified message to the client
                                        byte[] passwordSuccessSignedByte = signWithRSA("Succesfully Verified", 3072, SignKeyPair); // sign message
                                        byte[] message = Encoding.Default.GetBytes("Succesfully Verified");

                                        byte[] HMACacknowledgementByte = new byte[384 + message.Length];

                                        Array.Copy(passwordSuccessSignedByte, 0, HMACacknowledgementByte, 0, 384);
                                        Array.Copy(message, 0, HMACacknowledgementByte, 384, message.Length);

                                        sendMessage(HMACacknowledgementByte, newClient.Socket); // send message
                                        ServerConsol.AppendText("Username found\n");
                                        //*******************************************/*
                                        //BURADA CLIENT Secure Bir Şekilde SERVERA BAGLANICAK
                                        //**********************************************/

                                        // SESSION KEY GENERATION //*****************************************

                                        // Generate 128Bit random number
                                        byte[] SymEncDec128BitNum = new byte[16];
                                        using (var rng = new RNGCryptoServiceProvider())
                                        {
                                            rng.GetBytes(SymEncDec128BitNum);
                                        }

                                        byte[] SessionKey128BitNum = new byte[16];
                                        using (var rng = new RNGCryptoServiceProvider())
                                        {
                                            rng.GetBytes(SessionKey128BitNum);
                                        }
                                        byte[] sessionKeyMesByte = new byte[32];
                                        Array.Copy(SymEncDec128BitNum, 0, sessionKeyMesByte, 0, 16);
                                        Array.Copy(SessionKey128BitNum, 0, sessionKeyMesByte, 16, 16);
                                        byte[] sessionKeyDecryptedAES128 = decryptWithAES128(Encoding.Default.GetString(sessionKeyMesByte), Encoding.Default.GetBytes(pass), random128BitNumber);
       
                                        byte[] OK = Encoding.Default.GetBytes("OK");
                                        sessionKeyMesByte = new byte[32 + OK.Length];

                                        Array.Copy(sessionKeyDecryptedAES128, 0, sessionKeyMesByte, 0, 32);
                                        Array.Copy(OK, 0, sessionKeyMesByte, 32, OK.Length);
                                        
                                        //************************** SIGN THE SESSION KEY MESSAGE ******************************
                                        
                                        byte[] sessionKeyMesSignedByte = signWithRSA(Encoding.Default.GetString(sessionKeyMesByte), 3072, SignKeyPair); // sign message
                                        byte[] tempMessage = new byte[3072 + sessionKeyMesByte.Length];

                                        Array.Copy(sessionKeyMesByte, 0, tempMessage, 0, sessionKeyMesByte.Length);
                                        Array.Copy(sessionKeyMesSignedByte, 0, tempMessage, sessionKeyMesByte.Length, 3072); // concanated the session keys and sign

                                        sendMessage(tempMessage, newClient.Socket); // send session key message

                                        //*************************************************************************************

                                        newClient.Name = username;
                                        newClient.authenticationKey = SessionKey128BitNum;
                                        newClient.aesKey = SymEncDec128BitNum;

                                        Thread thread = new Thread(() => ReceiveFunc(newClient, SymEncDec128BitNum, SessionKey128BitNum));
                                        thread.Start();   
                                        socketList.Add(newClient);
                                    }
                                }
                                else   // send negatif acknowledgement message about Unsuccesfull HMAC Verification
                                {

                                    byte[] passwordSuccessSignedByte = signWithRSA("You CAN'T Verified", 3072, SignKeyPair); // sign message
                                    byte[] message = Encoding.Default.GetBytes("You CAN'T Verified");
                                    byte[] HMACacknowledgementByte = new byte[384 + message.Length];

                                    Array.Copy(passwordSuccessSignedByte, 0, HMACacknowledgementByte, 0, 384);
                                    Array.Copy(message, 0, HMACacknowledgementByte, 384, message.Length);

                                    sendMessage(HMACacknowledgementByte, newClient.Socket); // send message
                                }

                            }
                        }
                        sr.Close();
                        if (usernameFound == false)
                        {
                            ServerConsol.AppendText("In login username not fouund\n");
                        }
                    }
                }

            }
        }


        /*                      *\
         *      Cryptograpy     *
         * */


        // helper functions
        static string generateHexStringFromByteArray(byte[] input)
        {
            string hexString = BitConverter.ToString(input);
            return hexString.Replace("-", "");
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        //AES-128 Decryption
        static byte[] decryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            // aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try
            {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        // RSA decryption with varying bit length
        static byte[] decryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                result = rsaObject.Decrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // hash function: SHA-256
        static byte[] hashWithSHA256(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha256Hasher.ComputeHash(byteInput);

            return result;
        }


        static byte[] encryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                //true flag is set to perform direct RSA encryption using OAEP padding
                result = rsaObject.Encrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }


        static byte[] signWithRSA(string input, int algoLength, string xmlString)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            byte[] result = null;

            try
            {
                result = rsaObject.SignData(byteInput, "SHA256");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // HMAC with SHA-256
        static byte[] applyHMACwithSHA256(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA256 hmacSHA256 = new HMACSHA256(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA256.ComputeHash(byteInput);

            return result;
        }


    }
}