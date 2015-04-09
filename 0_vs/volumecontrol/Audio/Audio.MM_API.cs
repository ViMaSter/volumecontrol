using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Audio
{
    namespace MM_API
    {
        public struct Blob
        {
            public int Length;
            public IntPtr Data;

            //Code Should Compile at warning level4 without any warnings,
            //However this struct will give us Warning CS0649: Field [Fieldname]
            //is never assigned to, and will always have its default value
            //You can disable CS0649 in the project options but that will disable
            //the warning for the whole project, it's a nice warning and we do want
            //it in other places so we make a nice dummy function to keep the compiler
            //happy.
            private void FixCS0649()
            {
                Length = 0;
                Data = IntPtr.Zero;
            }
        }

        /// <summary>
        /// from Propidl.h.
        /// http://msdn.microsoft.com/en-us/library/aa380072(VS.85).aspx
        /// contains a union so we have to do an explicit layout
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct PropVariant
        {
            [FieldOffset(0)]
            short vt;
            [FieldOffset(2)]
            short wReserved1;
            [FieldOffset(4)]
            short wReserved2;
            [FieldOffset(6)]
            short wReserved3;
            [FieldOffset(8)]
            sbyte cVal;
            [FieldOffset(8)]
            byte bVal;
            [FieldOffset(8)]
            short iVal;
            [FieldOffset(8)]
            ushort uiVal;
            [FieldOffset(8)]
            int lVal;
            [FieldOffset(8)]
            uint ulVal;
            [FieldOffset(8)]
            int intVal;
            [FieldOffset(8)]
            uint uintVal;
            [FieldOffset(8)]
            long hVal;
            [FieldOffset(8)]
            long uhVal;
            [FieldOffset(8)]
            float fltVal;
            [FieldOffset(8)]
            double dblVal;
            [FieldOffset(8)]
            bool boolVal;
            [FieldOffset(8)]
            int scode;
            //CY cyVal;
            [FieldOffset(8)]
            DateTime date;
            [FieldOffset(8)]
            System.Runtime.InteropServices.ComTypes.FILETIME filetime;
            //CLSID* puuid;
            //CLIPDATA* pclipdata;
            //BSTR bstrVal;
            //BSTRBLOB bstrblobVal;
            [FieldOffset(8)]
            Blob blobVal;
            //LPSTR pszVal;
            [FieldOffset(8)]
            IntPtr pwszVal; //LPWSTR
            //IUnknown* punkVal;
            /*IDispatch* pdispVal;
            IStream* pStream;
            IStorage* pStorage;
            LPVERSIONEDSTREAM pVersionedStream;
            LPSAFEARRAY parray;
            CAC cac;
            CAUB caub;
            CAI cai;
            CAUI caui;
            CAL cal;
            CAUL caul;
            CAH cah;
            CAUH cauh;
            CAFLT caflt;
            CADBL cadbl;
            CABOOL cabool;
            CASCODE cascode;
            CACY cacy;
            CADATE cadate;
            CAFILETIME cafiletime;
            CACLSID cauuid;
            CACLIPDATA caclipdata;
            CABSTR cabstr;
            CABSTRBLOB cabstrblob;
            CALPSTR calpstr;
            CALPWSTR calpwstr;
            CAPROPVARIANT capropvar;
            CHAR* pcVal;
            UCHAR* pbVal;
            SHORT* piVal;
            USHORT* puiVal;
            LONG* plVal;
            ULONG* pulVal;
            INT* pintVal;
            UINT* puintVal;
            FLOAT* pfltVal;
            DOUBLE* pdblVal;
            VARIANT_BOOL* pboolVal;
            DECIMAL* pdecVal;
            SCODE* pscode;
            CY* pcyVal;
            DATE* pdate;
            BSTR* pbstrVal;
            IUnknown** ppunkVal;
            IDispatch** ppdispVal;
            LPSAFEARRAY* pparray;
            PROPVARIANT* pvarVal;
            */

            /// <summary>
            /// Helper method to gets blob data
            /// </summary>
            byte[] GetBlob()
            {
                byte[] Result = new byte[blobVal.Length];
                Marshal.Copy(blobVal.Data, Result, 0, Result.Length);
                return Result;
            }

            /// <summary>
            /// Property value
            /// </summary>
            public object Value
            {
                get
                {
                    VarEnum ve = (VarEnum)vt;
                    switch (ve)
                    {
                        case VarEnum.VT_I1:
                            return bVal;
                        case VarEnum.VT_I2:
                            return iVal;
                        case VarEnum.VT_I4:
                            return lVal;
                        case VarEnum.VT_I8:
                            return hVal;
                        case VarEnum.VT_INT:
                            return iVal;
                        case VarEnum.VT_UI4:
                            return ulVal;
                        case VarEnum.VT_LPWSTR:
                            return Marshal.PtrToStringUni(pwszVal);
                        case VarEnum.VT_BLOB:
                            return GetBlob();
                    }
                    throw new NotImplementedException("PropVariant " + ve.ToString());
                }
            }
        }

        /// <summary>
        /// Property Keys
        /// </summary>
        public static class PropertyKeys
        {
            /// <summary>
            /// PKEY_DeviceInterface_FriendlyName
            /// </summary>
            public static readonly Guid PKEY_DeviceInterface_FriendlyName = new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22);
            /// <summary>
            /// PKEY_Device_FriendlyName
            /// </summary>
            public static readonly Guid PKEY_Device_FriendlyName = new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0);
            /// <summary>
            /// PKEY_AudioEndpoint_FormFactor
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_FormFactor = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEndpoint_ControlPanelPageProvider
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_ControlPanelPageProvider = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEndpoint_Association
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_Association = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEndpoint_PhysicalSpeakers
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_PhysicalSpeakers = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEndpoint_GUID
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_GUID = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEndpoint_Disable_SysFx
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_Disable_SysFx = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEndpoint_FullRangeSpeakers
            /// </summary>
            public static readonly Guid PKEY_AudioEndpoint_FullRangeSpeakers = new Guid(0x1da5d803, 0xd492, 0x4edd, 0x8c, 0x23, 0xe0, 0xc0, 0xff, 0xee, 0x7f, 0x0e);
            /// <summary>
            /// PKEY_AudioEngine_DeviceFormat
            /// </summary>
            public static readonly Guid PKEY_AudioEngine_DeviceFormat = new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c);
        }

        /// <summary>
        /// PROPERTYKEY is defined in wtypes.h
        /// </summary>
        public struct PropertyKey
        {
            /// <summary>
            /// Format ID
            /// </summary>
            public Guid formatId;
            /// <summary>
            /// Property ID
            /// </summary>
            public int propertyId;
        }

        /// <summary>
        /// is defined in propsys.h
        /// </summary>
        [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyStore
        {
            int GetCount(out int propCount);
            int GetAt(int property, out PropertyKey key);
            int GetValue(ref PropertyKey key, out PropVariant value);
            int SetValue(ref PropertyKey key, ref PropVariant value);
            int Commit();
        }

        /// <summary>
        /// Property Store Property
        /// </summary>
        public class PropertyStoreProperty
        {
            private PropertyKey propertyKey;
            private PropVariant propertyValue;

            internal PropertyStoreProperty(PropertyKey key, PropVariant value)
            {
                propertyKey = key;
                propertyValue = value;
            }

            /// <summary>
            /// Property Key
            /// </summary>
            public PropertyKey Key
            {
                get
                {
                    return propertyKey;
                }
            }

            /// <summary>
            /// Property Value
            /// </summary>
            public object Value
            {
                get
                {
                    return propertyValue.Value;
                }
            }
        }

        /// <summary>
        /// Property Store class, only supports reading properties at the moment.
        /// </summary>
        public class PropertyStore
        {
            private IPropertyStore storeInterface;

            /// <summary>
            /// Property Count
            /// </summary>
            public int Count
            {
                get
                {
                    int result;
                    Marshal.ThrowExceptionForHR(storeInterface.GetCount(out result));
                    return result;
                }
            }

            /// <summary>
            /// Gets property by index
            /// </summary>
            /// <param name="index">Property index</param>
            /// <returns>The property</returns>
            public PropertyStoreProperty this[int index]
            {
                get
                {
                    PropVariant result;
                    PropertyKey key = Get(index);
                    Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
                    return new PropertyStoreProperty(key, result);
                }
            }

            /// <summary>
            /// Contains property guid
            /// </summary>
            /// <param name="guid">Looks for a specific Guid</param>
            /// <returns>True if found</returns>
            public bool Contains(Guid guid)
            {
                for (int i = 0; i < Count; i++)
                {
                    PropertyKey key = Get(i);
                    if (key.formatId == guid)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Indexer by guid
            /// </summary>
            /// <param name="guid">Property guid</param>
            /// <returns>Property or null if not found</returns>
            public PropertyStoreProperty this[Guid guid]
            {
                get
                {
                    PropVariant result;
                    for (int i = 0; i < Count; i++)
                    {
                        PropertyKey key = Get(i);
                        if (key.formatId == guid)
                        {
                            Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
                            return new PropertyStoreProperty(key, result);
                        }
                    }
                    return null;
                }
            }

            /// <summary>
            /// Gets property key at sepecified index
            /// </summary>
            /// <param name="index">Index</param>
            /// <returns>Property key</returns>
            public PropertyKey Get(int index)
            {
                PropertyKey key;
                Marshal.ThrowExceptionForHR(storeInterface.GetAt(index, out key));
                return key;
            }

            /// <summary>
            /// Gets property value at specified index
            /// </summary>
            /// <param name="index">Index</param>
            /// <returns>Property value</returns>
            public PropVariant GetValue(int index)
            {
                PropVariant result;
                PropertyKey key = Get(index);
                Marshal.ThrowExceptionForHR(storeInterface.GetValue(ref key, out result));
                return result;
            }

            /// <summary>
            /// Creates a new property store
            /// </summary>
            /// <param name="store">IPropertyStore COM interface</param>
            internal PropertyStore(IPropertyStore store)
            {
                this.storeInterface = store;
            }
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        internal class MMDeviceEnumerator
        {
        }

        public enum STATE
        {
            ACTIVE = 0x00000001,
            DISABLED = 0x00000002,
            NOTPRESENT = 0x00000004,
            UNPLUGGED = 0x00000008,
            MASK_ALL = 0x0000000F
        }

        public enum StorageAccessMode
        {
            Read = 0x00000000,
        }

        [Flags]
        public enum CLSCTX
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_ALL = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER
        }

        public enum EDataFlow
        {
            eRender,
            eCapture,
            eAll,
            EDataFlow_enum_count
        }

        public enum ERole
        {
            eConsole,
            eMultimedia,
            eCommunications,
            ERole_enum_count
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceEnumerator
        {
            [PreserveSig]
            int EnumAudioEndpoints(EDataFlow dataFlow, STATE dwStateMask, out IMMDeviceCollection ppDevices);

            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

            // the rest is not implemented
        }

        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceCollection
        {
            [PreserveSig]
            int GetCount(out uint pcDevices);
            [PreserveSig]
            int Item(uint nDevice, out IMMDevice Device);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

            [PreserveSig]
            int OpenPropertyStore(StorageAccessMode stgmAccess, out IPropertyStore ppProperties);

            [PreserveSig]
            int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

            [PreserveSig]
            int GetState(out STATE pdwState);
        }

        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionManager2
        {
            [PreserveSig]
            int GetAudioSessionControl([MarshalAs(UnmanagedType.LPStruct)] Guid AudioSessionGuid, int StreamFlags, out IAudioSessionControl SessionControl);

            [PreserveSig]
            int GetSimpleAudioVolume([MarshalAs(UnmanagedType.LPStruct)] Guid AudioSessionGuid, int StreamFlags, ISimpleAudioVolume AudioVolume);

            [PreserveSig]
            int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

            [PreserveSig]
            int RegisterSessionNotification(IAudioSessionNotification SessionNotification);

            [PreserveSig]
            int UnregisterSessionNotification(IAudioSessionNotification SessionNotification);

            int RegisterDuckNotificationNotImpl();
            int UnregisterDuckNotificationNotImpl();
        }

        [Guid("641DD20B-4D41-49CC-ABA3-174B9477BB08"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionNotification
        {
            void OnSessionCreated(IAudioSessionControl NewSession);
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionEnumerator
        {
            [PreserveSig]
            int GetCount(out int SessionCount);

            [PreserveSig]
            int GetSession(int SessionCount, out IAudioSessionControl Session);
        }

        [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionControl
        {
            int NotImpl1();

            [PreserveSig]
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            int NotImpl2();

            [PreserveSig]
            int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
            // the rest is not implemented
        }

        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISimpleAudioVolume
        {
            [PreserveSig]
            int SetMasterVolume(float fLevel, ref Guid EventContext);

            [PreserveSig]
            int GetMasterVolume(out float pfLevel);

            [PreserveSig]
            int SetMute(bool bMute, ref Guid EventContext);

            [PreserveSig]
            int GetMute(out bool pbMute);
        }


        public enum AudioSessionState
        {
            AudioSessionStateInactive = 0,
            AudioSessionStateActive = 1,
            AudioSessionStateExpired = 2
        }

        public enum AudioSessionDisconnectReason
        {
            DisconnectReasonDeviceRemoval = 0,
            DisconnectReasonServerShutdown = (DisconnectReasonDeviceRemoval + 1),
            DisconnectReasonFormatChanged = (DisconnectReasonServerShutdown + 1),
            DisconnectReasonSessionLogoff = (DisconnectReasonFormatChanged + 1),
            DisconnectReasonSessionDisconnected = (DisconnectReasonSessionLogoff + 1),
            DisconnectReasonExclusiveModeOverride = (DisconnectReasonSessionDisconnected + 1)
        }

        [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionEvents
        {
            [PreserveSig]
            int OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, Guid EventContext);
            [PreserveSig]
            int OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, Guid EventContext);
            [PreserveSig]
            int OnSimpleVolumeChanged(float NewVolume, bool newMute, Guid EventContext);
            [PreserveSig]
            int OnChannelVolumeChanged(UInt32 ChannelCount, IntPtr NewChannelVolumeArray, UInt32 ChangedChannel, Guid EventContext);
            [PreserveSig]
            int OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext);
            [PreserveSig]
            int OnStateChanged(AudioSessionState NewState);
            [PreserveSig]
            int OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
        }

        [Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioSessionControl2
        {
            [PreserveSig]
            int GetState(out AudioSessionState state);
            [PreserveSig]
            int GetDisplayName([Out(), MarshalAs(UnmanagedType.LPWStr)] out string name);
            [PreserveSig]
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, Guid EventContext);
            [PreserveSig]
            int GetIconPath([Out(), MarshalAs(UnmanagedType.LPWStr)] out string Path);
            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, Guid EventContext);
            [PreserveSig]
            int GetGroupingParam(out Guid GroupingParam);
            [PreserveSig]
            int SetGroupingParam(Guid Override, Guid Eventcontext);
            [PreserveSig]
            int RegisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int UnregisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
            [PreserveSig]
            int GetSessionIdentifier([Out(), MarshalAs(UnmanagedType.LPWStr)] out string retVal);
            [PreserveSig]
            int GetSessionInstanceIdentifier([Out(), MarshalAs(UnmanagedType.LPWStr)] out string retVal);
            [PreserveSig]
            int GetProcessId(out UInt32 retvVal);
            [PreserveSig]
            int IsSystemSoundsSession();
            [PreserveSig]
            int SetDuckingPreference(bool optOut);
        }
    }
}