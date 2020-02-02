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
using System.IO;
using System.Security.Cryptography;

namespace Cs432_projectStep1_client_0._01
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button_connect_Click(object sender, EventArgs e)  // LOGIN BUTTON
        {
            Thread LoginThread = new Thread(new ThreadStart(LoginPhase));
            LoginThread.Start();
        }

        private void LoginPhase()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_IP.Text;
            textBox_IP.Text = "";
            int port;
            string EncDecPublic;
            string signVerPublic;
            bool verificationResult = false;
            ConsoleServer.AppendText("Connecting to the server");
            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(@"C:\server_enc_dec_pub.txt"))
            {
                EncDecPublic = fileReader.ReadLine();
            }
            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(@"C:\server_signing_verification_pub.txt"))
            {
                signVerPublic = fileReader.ReadLine();
            }
            //if(string.IsNullOrWhiteSpace(textBox_IP.Text) && string)
            if (Int32.TryParse(textBox_Port.Text, out port))
            {
                try
                {
                    clientSocket.Connect(IP, port);
                }
                catch
                {
                    ConsoleServer.AppendText("Connection is not succesfull\nRe-enter Port and IP\n");
                }

                /*
                 *          LOGING
                 * sending username and password with sha 
                 * 
                 * */

                byte[] loginReqEncByte = encryptWithRSA("login", 3072, EncDecPublic);
                try
                {
                    
                    sendMessage(Encoding.Default.GetString(loginReqEncByte)); // sends login request to the server
                }
                catch
                {
                    ConsoleServer.AppendText("A problem occured sending message ERROR0.001\n");
                }
                //byte[] sha256 = hashWithSHA256(textBox_Password.Text);

                /********** AUTHENTICATION REQUEST TO THE SERVER TOGETHER WITH USERNAME **************/

                byte[] hashOfUsername = hashWithSHA256(textBox_UserName.Text);
                byte[] usernameByte = Encoding.Default.GetBytes(textBox_UserName.Text);

                byte[] concatenatedByte = new byte[32 + usernameByte.Length];
                Array.Copy(hashOfUsername, 0, concatenatedByte, 0, 32);
                Array.Copy(usernameByte, 0, concatenatedByte, 32, usernameByte.Length);

                byte[] authenticationRequest = encryptWithRSA(Encoding.Default.GetString(concatenatedByte), 3072, EncDecPublic);

                try
                {
                    sendMessage(Encoding.Default.GetString(authenticationRequest));         //sends Challange Response protochols result SEEEND
                }
                catch
                {
                    ConsoleServer.AppendText("A problem occured sending message ERROR0.002\n");
                }


                /*******************************************************************************/

                //Server sends the 128bit random number 
                string serversMessage = "";

                try
                {
                    serversMessage = receiveMessage();          

                }
                catch
                {
                    ConsoleServer.AppendText("A problem occured while Receiving message ERROR0.003\n");
                }
                byte[] receivedBytes = Encoding.Default.GetBytes(serversMessage);
                byte[] hashOfRandomNum = new byte[32];
                byte[] random128BitNum = new byte[16];
                Array.Copy(receivedBytes, 0, hashOfRandomNum, 0, 32);
                Array.Copy(receivedBytes, 32, random128BitNum, 0, 16);
                if (Encoding.Default.GetString(hashWithSHA256(Encoding.Default.GetString(random128BitNum))) == Encoding.Default.GetString(hashOfRandomNum)) // Yollanan random number kazasız belasız ulaşmış mı ?
                {
                    byte[] hashOfPassword = hashWithSHA256(textBox_Password.Text);
                    byte[] upperhalfOfPassword = new byte[16];
                    Array.Copy(hashOfPassword, 0, upperhalfOfPassword, 0, 16);

                    byte[] hmacsha256 = applyHMACwithSHA256(Encoding.Default.GetString(random128BitNum), upperhalfOfPassword);
                    try
                    {
                       
                        sendMessage(Encoding.Default.GetString(hmacsha256));
                    }
                    catch
                    {
                        ConsoleServer.AppendText("A problem occured while Sending message ERROR0.004\n");
                    }
                    // SIGNED ACKNOWLEDGEMENT RECEIVED FROM SERVER AFTER HMAC AUTHENTICATION 
                    try
                    {
                        serversMessage = receiveMessage();
                    }
                    catch
                    {
                        ConsoleServer.AppendText("A problem occured while receiving message ERROR0.005\n");
                    }
                    receivedBytes = Encoding.Default.GetBytes(serversMessage);
                    byte[] signOfAcknowledgement = new byte[384];
                    byte[] acknowledgementMessage = new byte[receivedBytes.Length - 384];
                    Array.Copy(receivedBytes, 0, signOfAcknowledgement, 0, 384);
                    Array.Copy(receivedBytes, 384, acknowledgementMessage, 0, receivedBytes.Length - 384);

                    //TRY TO VERIFY SIGNED MESSAGE WHETHER THE SERVER IS VALID 
                    verificationResult = verifyWithRSA(Encoding.Default.GetString(acknowledgementMessage), 3072, signVerPublic, signOfAcknowledgement);
                    if (verificationResult == true)
                    {
                        ConsoleServer.AppendText("Login Acknowledgement Comes From VALID Server\n");
                        if (Encoding.Default.GetString(acknowledgementMessage) == "Succesfully Verified")
                        {
                            //***************** ARTIK GİRİS YAPABİLİRSİN DOSTUM ******************//
                            ConsoleServer.AppendText("You are SUCCESFULLY Connectected to the Server\n");
                            ConsoleServer.AppendText("You are now Logged in\n");
                            button_connect.Enabled = false;
                            button_Enroll.Enabled = false;
                            connected = true;
                            //GET SESSION KEYS FROM SERVER PHASE
                            serversMessage = receiveMessage();

                            receivedBytes = Encoding.Default.GetBytes(serversMessage);
                            byte[] sessionKeyMesSign = new byte[384];
                            byte[] sessionKeyMes = new byte[receivedBytes.Length - 384];
                            Array.Copy(receivedBytes, 0, sessionKeyMes, 0, receivedBytes.Length-384);
                            Array.Copy(receivedBytes, 384, sessionKeyMesSign, receivedBytes.Length - 384, 384);

                            verificationResult = verifyWithRSA(serversMessage, 3072, signVerPublic, sessionKeyMesSign);
                            if (verificationResult == true)
                            {
                                byte[] SessionkeysEncrypted = new byte[32];
                                Array.Copy(SessionkeysEncrypted, 0, sessionKeyMes, 0, 32); // Seperate session key from "OK"
                                byte[] Sessionkeys = decryptWithAES128(Encoding.Default.GetString(SessionkeysEncrypted), upperhalfOfPassword, random128BitNum); // Decrypte session key using AES128

                                byte[] SymEncDec128BitNum = new byte[16]; // First 128Bit is AES keys
                                byte[] SessionKey128BitNum = new byte[16]; // Second 128Bit is Authentcation Key for Session
                                Array.Copy(Sessionkeys, 0, SymEncDec128BitNum, 0, 16);
                                Array.Copy(Sessionkeys, 16, SessionKey128BitNum, 0, 16);

                                Thread thread = new Thread(() => ReceiveFunc(SymEncDec128BitNum, SessionKey128BitNum)); // Serverın yollayacakları için ReceiveFunc Thread i başlatılıyor 
                                thread.Start();

                            }                          
                        }
                        else if(Encoding.Default.GetString(acknowledgementMessage) == "Succesfully Verified But You are Allready In Server")
                            //************ VERIFY EDİLEN KULLANICI ZATEN SERVER DA DOSTUM ********//
                            ConsoleServer.AppendText("You are SUCCESFULLY Verified by the Server but BRO You are Allready In the Server \n Please Try again\n");
                        else if (Encoding.Default.GetString(acknowledgementMessage) == "You CAN'T Verified")
                            ConsoleServer.AppendText("You CAN'T Login The System \n Please Try again\n");
                    }
                    else
                        ConsoleServer.AppendText("Login Acknowledgement Comes From INVALID Server\n");
                }
            }
            else
            {
                textBox_Port.Text = "";
                ConsoleServer.AppendText("Check the port\n");
            }
        }



       

        private void button_Enroll_Click(object sender, EventArgs e)
        {
            Thread enrollThread = new Thread(new ThreadStart(EnrollPhase));
            enrollThread.Start();
        }

        private void EnrollPhase()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_IP.Text;
            textBox_IP.Text = "";

            int port;
            string EncDecPublic;
            string signVerPublic;

            ConsoleServer.AppendText("Connecting\n");
            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(@"C:\server_enc_dec_pub.txt"))
            {
                EncDecPublic = fileReader.ReadLine();
            }
            using (System.IO.StreamReader fileReader = new System.IO.StreamReader(@"C:\server_signing_verification_pub.txt"))
            {
                signVerPublic = fileReader.ReadLine();
            }
            //if(string.IsNullOrWhiteSpace(textBox_IP.Text) && string)
            if (Int32.TryParse(textBox_Port.Text, out port))
            {
                try
                {
                    clientSocket.Connect(IP, port);
                }
                catch
                {
                    ConsoleServer.AppendText("Connection is not succesfull\nRe-enter Port and IP\n");
                }
                ConsoleServer.AppendText("Connected to the server\n");
                /*
                 *              ENROLLMENT 
                 * sending username and password with sha 
                 * 
                 * */

                ConsoleServer.AppendText("Enrolling to the server\n");


                byte[] loginReqByte = Encoding.Default.GetBytes("enroll");
                byte[] loginReqEncByte = encryptWithRSA(Encoding.Default.GetString(loginReqByte), 3072, EncDecPublic);
                sendMessage(Encoding.Default.GetString(loginReqEncByte)); // sends ENROLL request to the server

                byte[] passwordHash = hashWithSHA256(textBox_Password.Text); // Girilen PASSWORD 
                byte[] usernameByte = Encoding.Default.GetBytes(textBox_UserName.Text); // Girilen USERNAME 
                byte[] concatenatedByte = new byte[16 + textBox_UserName.Text.Length];

                Array.Copy(passwordHash, 0, concatenatedByte, 0, 16);
                Array.Copy(usernameByte, 0, concatenatedByte, 16, textBox_UserName.Text.Length);

                byte[] encryptedRSA = encryptWithRSA(Encoding.Default.GetString(concatenatedByte), 3072, EncDecPublic);
                sendMessage(Encoding.Default.GetString(encryptedRSA)); // Girilen PASSWORD ve  USERNAME ikilisi birleştirilip yollanır

                /*/*//* CLIENT GET THE SIGNED RESPONSE FROM SERVER ABOUT ENROLMENT CHECK WHETHER IT IS VALID SERVER OR NOT *//*/*/

                string serversMessage = receiveMessage();

                byte[] receivedBytes = Encoding.Default.GetBytes(serversMessage);
                byte[] ServerSignedByte = new byte[384];
                byte[] receivedMessage = new byte[receivedBytes.Length - 384];

                Array.Copy(receivedBytes, 0, ServerSignedByte, 0, 384);
                Array.Copy(receivedBytes, 384, receivedMessage, 0, receivedBytes.Length - 384);
                //string messss = Encoding.Default.GetString(receivedMessage);
                bool verificationResult = verifyWithRSA(Encoding.Default.GetString(receivedMessage), 3072, signVerPublic, ServerSignedByte);
                if (verificationResult == true)
                {
                    ConsoleServer.AppendText("Enrollment Message Come From VALID Server\n");
                    if (Encoding.Default.GetString(receivedMessage) == "Succesfully Enrolled")
                    {
                        ConsoleServer.AppendText("You are succesfully enrolled the system\n");
                        //***************** ARTIK GİRİS YAPABİLİRSİN DOSTUM ******************//
                        button_connect.Enabled = false;
                        button_Enroll.Enabled = false;
                        connected = true;
                        ConsoleServer.AppendText("Connectected to the server\n");


                        //Thread receiveThread = new Thread(new ThreadStart(Receive));
                        //receiveThread.Start();

                        //textBox_Password.Text = "";

                        
                    }
                    else if (Encoding.Default.GetString(receivedMessage) == "Error!Username Exists")
                    {
                        ConsoleServer.AppendText("Username exist in the system! Please try again with different username\n");
                        //********************* TRY AGAIN PROCESS BURAYA GELICEK **************//

                        textBox_Password.Text = "";
                        textBox_UserName.Text = "";
                        

                    }
                    else
                    {
                        ConsoleServer.AppendText("Message from server has a problem \n");
                    }

                }
                else
                {
                    ConsoleServer.AppendText("Enrollment Message Come From UNVALID Server\n");
                }

                //////////////////////////////////////////////

               
            }
            else
            {
                textBox_Port.Text = "";
                ConsoleServer.AppendText("Check the port\n");
            }
        }


        private void button_sendMessage_Click(object sender, EventArgs e)
        {
            sendMessage(ClientInput_Console.Text);
            ClientInput_Console.Text = "";
        }

        private void ReceiveFunc(byte[] aesEncDecKey, byte[] authentKey) // SESSION KEY PARAMETRE OLARAK ALINMALI ??
        {
            bool myThreadTerminating = false;
            while (!myThreadTerminating)
            {
                try
                {
                    string myMessage = receiveMessage();
                    
                }
                catch
                {
                    ConsoleServer.AppendText("A problem occured while receiving message\n");
                    myThreadTerminating = true;
                }

            }
        }




        /*
         * *
         * *
         * *
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * */

        private void sendMessage(string m)
        {
            Thread.Sleep(500);
            Byte[] a = new Byte[100];
            a = Encoding.Default.GetBytes(m.Length.ToString());
            clientSocket.Send(a);

            a = new Byte[m.Length];
            a = Encoding.Default.GetBytes(m);
            clientSocket.Send(a);
        }

        private string receiveMessage()
        {
            string m;
            int byteLength;
            byte[] a = new byte[10000];
            clientSocket.Receive(a);
            Int32.TryParse(Encoding.Default.GetString(a), out byteLength);

            a = new byte[byteLength];
            clientSocket.Receive(a);
            m = Encoding.Default.GetString(a);

            return m;
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

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

        // encryption with AES-128
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


        // verifying with RSA
        static bool verifyWithRSA(string input, int algoLength, string xmlString, byte[] signature)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try
            {
                result = rsaObject.VerifyData(byteInput, "SHA256", signature);
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
            // get the result of HMAC operationB
            byte[] result = hmacSHA256.ComputeHash(byteInput);

            return result;
        }

        
    }
}
