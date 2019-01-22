//    Source string:
//JMapCheckStation2017

public class EncryptedString1
{
    public const int EncryptBufFeatureID = 1;    //feature id which is selected
    public int SourceBufLen = 20;    //length of source string
    public int EncryptBufLen = 20;    //length of encrypt string

    public int isString = 1;    //This is a string buffer
/*The encrypted array is in UTF-8 format. Please convert it to proper format before using it.*/ 

    public byte[] encryptStrArr = new byte[20]{ 
   0x24, 0x0B, 0xA6, 0xBD, 0x8E, 0xDA, 0x6D, 0x94, 0xCB, 0x13, 0xFD, 0x2D, 0x7B, 0xE0, 0xF4, 0x4A, 0x46, 0xC2, 0x05, 0xCC
 };
}