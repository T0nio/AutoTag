using System;
using System.Runtime.InteropServices;

namespace Recognizer
{
    class ACRCloudExtrTool
    {
        public ACRCloudExtrTool()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                acr_recognize_global_init();
            }
            else
            {
                acr_init();
            }
        }


        /**
          *
          *  create "ACRCloud Fingerprint" by file path of (Audio/Video file)
          *          Audio: mp3, wav, m4a, flac, aac, amr, ape, ogg ...
          *          Video: mp4, mkv, wmv, flv, ts, avi ...
          *
          *  @param filePath query file path
          *  @param startTimeSeconds skip (startSeconds) seconds from from the beginning of (filePath)
          *  @param audioLenSeconds Length of audio data you need. if you create recogize frigerprint, default is 12 seconds, if you create db frigerprint, it is not usefully; 
          *  @param isDB   If it is True, it will create db frigerprint; 
          *  
          *  @return result "ACRCloud Fingerprint"
          *
          **/
        public byte[] CreateFingerprintByFile(string filePath, int startTimeSeconds, int audioLenSeconds, bool isDB)
        {
            byte[] fpBuffer = null;

            byte tIsDB = (isDB) ? (byte)1 : (byte)0;
            IntPtr pFpBuffer = IntPtr.Zero;
            
            int fpBufferLen;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                fpBufferLen = acr_create_fingerprint_by_file(filePath, startTimeSeconds, audioLenSeconds, tIsDB, ref pFpBuffer);
            }
            else
            {
                fpBufferLen = create_fingerprint_by_file(filePath, startTimeSeconds, audioLenSeconds, tIsDB, ref pFpBuffer);
            }
 
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception(filePath + " is not readable!");
                case -2:
                    throw new Exception(filePath + " can not be decoded audio data!");
            }
            if (fpBufferLen == 0)
            {
                return fpBuffer;
            }         

            fpBuffer = new byte[fpBufferLen];
            Marshal.Copy(pFpBuffer, fpBuffer, 0, fpBufferLen);
            acr_free(pFpBuffer);

            return fpBuffer;
        }


        /**
          *
          *  decode audio from file path of (Audio/Video file)
          *          Audio: mp3, wav, m4a, flac, aac, amr, ape, ogg ...
          *          Video: mp4, mkv, wmv, flv, ts, avi ...
          *
          *  @param filePath query file path
          *  @param startTimeSeconds skip (startSeconds) seconds from from the beginning of (filePath)
          *  @param audioLenSeconds Length of audio data you need, if it is 0, will decode all the audio;  
          *  
          *  @return result audio data(formatter:RIFF (little-endian) data, WAVE audio, Microsoft PCM, 16 bit, mono 8000 Hz)
          *
          **/
        public byte[] DecodeAudioByFile(string filePath, int startTimeSeconds, int audioLenSeconds)
        {
            byte aa = 1;
            acr_set_debug(aa);
            byte[] audioBuffer = null;

            IntPtr pAudioBuffer = IntPtr.Zero;
            int fpBufferLen = decode_audio_by_file(filePath, startTimeSeconds, audioLenSeconds, ref pAudioBuffer);
            switch (fpBufferLen)
            {
                case -1:
                    throw new Exception(filePath + " is not readable!");
                case -2:
                    throw new Exception(filePath + " can not be decoded audio data!");
            }
            if (fpBufferLen == 0)
            {
                return audioBuffer;
            }

            audioBuffer = new byte[fpBufferLen];
            Marshal.Copy(pAudioBuffer, audioBuffer, 0, fpBufferLen);
            acr_free(pAudioBuffer);

            return audioBuffer;
        }

        

        /**
          *
          *  get duration from file buffer of (Audio/Video file)
          *          Audio: mp3, wav, m4a, flac, aac, amr, ape, ogg ...
          *          Video: mp4, mkv, wmv, flv, ts, avi ...
          *
          *  @param filePath query file path 
          *  
          *  @return duration ms
          *
          **/
        public int GetDurationMillisecondByFile(string filePath)
        {
            return get_duration_ms_by_file(filePath);
        } 

        [DllImport("libacrcloud_extr_tool")]
        private static extern int acr_create_fingerprint_by_file(string file_path, int start_time_seconds, int audio_len_seconds, byte is_db_fingerprint, ref IntPtr fps_buffer);
        [DllImport("libacrcloud_extr_tool")]
        private static extern int create_fingerprint_by_file(string file_path, int start_time_seconds, int audio_len_seconds, byte is_db_fingerprint, ref IntPtr fps_buffer);
        [DllImport("libacrcloud_extr_tool")]
        private static extern int decode_audio_by_file(string file_path, int start_time_seconds, int audio_len_seconds, ref IntPtr audio_buffer);
        [DllImport("libacrcloud_extr_tool")]
        private static extern void acr_free(IntPtr buffer);
        [DllImport("libacrcloud_extr_tool")]
        private static extern int get_duration_ms_by_file(string file_path);
        [DllImport("libacrcloud_extr_tool")]
        public static extern void acr_set_debug(byte is_debug);
        [DllImport("libacrcloud_extr_tool")]
        private static extern int acr_recognize_global_init();
        [DllImport("libacrcloud_extr_tool")]
        private static extern void acr_init();
    }

}