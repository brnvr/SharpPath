using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpPath.GameMaker
{
    public enum EventType
    {
        EVENT_OTHER_WEB_IMAGE_LOAD = 60,
        EVENT_OTHER_WEB_SOUND_LOAD = 61,
        EVENT_OTHER_WEB_ASYNC = 62,
        EVENT_OTHER_DIALOG_ASYNC = 63,
        EVENT_OTHER_WEB_IAP = 66,
        EVENT_OTHER_WEB_CLOUD = 67,
        EVENT_OTHER_WEB_NETWORKING = 68,
        EVENT_OTHER_WEB_STEAM = 69,
        EVENT_OTHER_SOCIAL = 70,
        EVENT_OTHER_PUSH_NOTIFICATION = 71,
        EVENT_OTHER_ASYNC_SAVE_LOAD = 72,
        EVENT_OTHER_AUDIO_RECORDING = 73,
        EVENT_OTHER_AUDIO_PLAYBACK = 74,
        EVENT_OTHER_SYSTEM_EVENT = 75,
        EVENT_OTHER_MESSAGE_EVENT = 76
    }

    public class GMExtension
    {
        public delegate void EventPerformAsyncDelegate(int map, int event_type);
        public delegate int DsMapCreateDelegate(int n);
        public delegate bool DsMapAddDoubleDelegate(int map, string key, double value);
        public delegate bool DsMapAddStringDelegate(int map, string key, string value);

        EventPerformAsyncDelegate eventPerformAsync;
        DsMapCreateDelegate dsMapCreate;
        DsMapAddDoubleDelegate dsMapAddDouble;
        DsMapAddStringDelegate dsMapAddString;

        public unsafe GMExtension(char* arg0, char* arg1, char* arg2, char* arg3)
        {
            eventPerformAsync = Marshal.GetDelegateForFunctionPointer<EventPerformAsyncDelegate>(new IntPtr(arg0));
            dsMapCreate = Marshal.GetDelegateForFunctionPointer<DsMapCreateDelegate>(new IntPtr(arg1));
            dsMapAddDouble = Marshal.GetDelegateForFunctionPointer<DsMapAddDoubleDelegate>(new IntPtr(arg2));
            dsMapAddString = Marshal.GetDelegateForFunctionPointer<DsMapAddStringDelegate>(new IntPtr(arg3));
        }

        public void EventPerformAsync(int map, EventType type)
        {
            eventPerformAsync(map, (int)type);
        }

        public int DsMapCreate(int n = 0)
        {
            return dsMapCreate(n);
        }

        public bool DsMapAdd(int map, string key, double value)
        {
            return dsMapAddDouble(map, key, value);
        }

        public bool DsMapAdd(int map, string key, string value)
        {
            return dsMapAddString(map, key, value);
        }

        public bool DsMapAdd(int map, Dictionary<string, string> values)
        {
            foreach (KeyValuePair<string, string> value in values)
            {
                if (!DsMapAdd(map, value.Key, value.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DsMapAdd(int map, Dictionary<string, double> values)
        {
            foreach (KeyValuePair<string, double> value in values)
            {
                if (!DsMapAdd(map, value.Key, value.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public static unsafe string StringFromCharPointer(char* charPointer)
        {
            return Marshal.PtrToStringAnsi((IntPtr)charPointer);
        }

        public static byte[] ToByteArray(List<char> values)
        {
            return ToByteArray(values, value => BitConverter.GetBytes(value));
        }

        public static byte[] ToByteArray(List<double> values)
        {
            return ToByteArray(values, value => BitConverter.GetBytes(value));
        }

        static byte[] ToByteArray<T>(List<T> values, Func<T, byte[]> getBytes) where T : struct
        {
            int typeSize;
            byte[] byteArr;

            typeSize = sizeof(int);
            byteArr = new byte[typeSize * values.Count];

            for (var i = 0; i < values.Count; i++)
            {
                var a = getBytes(values[i]);

                a.CopyTo(byteArr, i * typeSize);
            }

            return byteArr;
        }
    }
}

