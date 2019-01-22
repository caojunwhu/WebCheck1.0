////////////////////////////////////////////////////////////////////
// Demo program for SuperDog licensing functions
//
// Copyright (C) 2013 SafeNet, Inc. All rights reserved.
//
// SuperDog(R) is a trademark of SafeNet, Inc.
//
////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using SuperDog;
using System.Windows.Forms;

namespace Authentication.Class
{
   public class SuperDogChecker
    {
        public static EncryptedString1 encrString1 = new EncryptedString1();
       string scope = "<dogscope />";
        static int ExpiredDateFeatureID = 3;

        public DogStatus DecryptString()
        {
            DogStatus status;
            UInt32 i = 0;
            byte[] bufData;
            byte[] strTmp;
            string strContents; 

            Dog curDog = new Dog(new DogFeature(DogFeature.FromFeature(EncryptedString1.EncryptBufFeatureID).Feature));

            /************************************************************************
             * Login
             *   establishes a context for SuperDog
             */
            status = curDog.Login(VendorCode.strVendorCode, scope); 
            if (status != DogStatus.StatusOk)
            {
                if (status == DogStatus.InvalidVendorCode)
                {
                    MessageBox.Show("Invalid vendor code.\n");
                }
                else if (status == DogStatus.UnknownVcode)
                {
                    MessageBox.Show("Vendor Code not recognized by API.\n");
                }
                else
                {
                    MessageBox.Show("Login to feature failed with status: " + status);
                }
                return status;
            }

            bufData = new byte[encrString1.EncryptBufLen];
            for (i = 0; i < encrString1.EncryptBufLen; ++i)
            {
                bufData[i] = encrString1.encryptStrArr[i];
            }

            // decrypt the data.
            // on success we convert the data back into a 
            // human readable string.
            status = curDog.Decrypt(bufData);
            if (DogStatus.StatusOk != status)
            {
                MessageBox.Show("Dog decrypt failed with status: " + status);
                curDog.Logout();
                return status;
            }
            
            //If source string length is less than 16, we need cut the needless buffer
            if (encrString1.EncryptBufLen > encrString1.SourceBufLen)
            {
                strTmp = new byte[encrString1.SourceBufLen];
                for (i = 0; i < encrString1.SourceBufLen; ++i)
                {
                    strTmp[i] = bufData[i];
                }
                strContents = UTF8Encoding.UTF8.GetString(strTmp);
            }
            else
            {
                strContents = UTF8Encoding.UTF8.GetString(bufData);
            }

            //Use the decrypted string do some operation    
            if (0 == encrString1.isString)
            {
                DumpBytes(bufData, encrString1.SourceBufLen);
            }
            else
            { 
                //MessageBox.Show("The decrypted string is: \"" + strContents + "\".");
                if (strContents.IndexOf("JMapCheckStation2017") < 0)
                    return DogStatus.AccessDenied;

            } 

            status = curDog.Logout();
            return status;
        }

        public DogStatus CheckExpiredDate()
        {
            return CheckExpiredDate(ExpiredDateFeatureID);
        }
        public DogStatus CheckExpiredDate(int ExpiredDateFeatureID)
        {
            DogStatus status;

            Dog curDog = new Dog(new DogFeature(DogFeature.FromFeature(ExpiredDateFeatureID).Feature));

            /************************************************************************
             * Login
             *   establishes a context for SuperDog
             */
            status = curDog.Login(VendorCode.strVendorCode, scope);
            if (status != DogStatus.StatusOk)
            {
                if (status == DogStatus.InvalidVendorCode)
                {
                    MessageBox.Show("Invalid vendor code.\n");
                }
                else if (status == DogStatus.UnknownVcode)
                {
                    MessageBox.Show("Vendor Code not recognized by API.\n");
                }
                else
                {
                    MessageBox.Show("Login to feature failed with status: " + status);
                }
                return status;
            }

            status = curDog.Logout();
            return status;
        }

        public DateTime GetCurrentDate()
        {
            DogStatus status;
            DateTime dt = DateTime.Now;
            Dog curDog = new Dog(new DogFeature(DogFeature.FromFeature(ExpiredDateFeatureID).Feature));
            status = curDog.Login(VendorCode.strVendorCode, scope);
            if (status != DogStatus.StatusOk)
            {
                if (status == DogStatus.InvalidVendorCode)
                {
                    MessageBox.Show("Invalid vendor code.\n");
                }
                else if (status == DogStatus.UnknownVcode)
                {
                    MessageBox.Show("Vendor Code not recognized by API.\n");
                }
                else
                {
                    MessageBox.Show("Login to feature failed with status: " + status);
                }
                return dt;
            }
            curDog.GetTime(ref dt);
            return dt;

        }

        public string GetExpiredDate()
        {
            //DateTime dt = DateTime.Now;
            string date = DateTime.Now.ToShortDateString();
            DogStatus status;
            UInt32 i = 0;
            byte[] bufData;
            byte[] strTmp;
            string strContents;
            int ExpiredDateLength = 128;
            int FileID = 65524;

            Dog curDog = new Dog(new DogFeature(DogFeature.FromFeature(ExpiredDateFeatureID).Feature));

            /************************************************************************
             * Login
             *   establishes a context for SuperDog
             */
            status = curDog.Login(VendorCode.strVendorCode, scope);
            if (status != DogStatus.StatusOk)
            {
                if (status == DogStatus.InvalidVendorCode)
                {
                    MessageBox.Show("Invalid vendor code.\n");
                }
                else if (status == DogStatus.UnknownVcode)
                {
                    MessageBox.Show("Vendor Code not recognized by API.\n");
                }
                else
                {
                    MessageBox.Show("Login to feature failed with status: " + status);
                }
                return date;
            }

            DogFile file = curDog.GetFile(FileID);
            //status = file.FileSize(ref ExpiredDateLength);
            bufData = new byte[ExpiredDateLength];
            status = file.Read(bufData, 0, ExpiredDateLength);
            // decrypt the data.
            // on success we convert the data back into a 
            // human readable string.
            //status = curDog.Decrypt(bufData);
            if (DogStatus.StatusOk != status)
            {
                MessageBox.Show("Dog decrypt failed with status: " + status);
                curDog.Logout();
                return date;
            }
            strTmp = new byte[8];
            for (i = 0; i < 8; ++i)
            {
                strTmp[i] = bufData[i];
            }

            strContents = UTF8Encoding.UTF8.GetString(strTmp);
            date = strContents;
            return date;
        }


        /// <summary>
        /// Dumps a bunch of bytes into the referenced TextBox.
        /// </summary>
        protected void DumpBytes(byte[] bytes, int ilen)
        {
            MessageBox.Show("The decrypted buffer data is below :");
            int index = 0;

            for (index = 0; index < ilen; index++)
            {
                if (0 == (index % 8))
                {
                    if (0 == index)
                    {
                        MessageBox.Show("          ");
                    }
                    else
                    {
                        MessageBox.Show("\r\n          "); 
                    } 
                } 
                MessageBox.Show("0x" + bytes[index].ToString("X2") + " "); 
            }
            MessageBox.Show("");
        }

    }
}