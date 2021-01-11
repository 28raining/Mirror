// wraps Telepathy for use as HLAPI TransportLayer
using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Runtime.InteropServices;

// Transport using peerJS (webRTC)
namespace Mirror
{
    [DisallowMultipleComponent]
    public class peerJSTransport : Transport
    {
        [Header("Server")]
        [Tooltip("Protect against allocation attacks by keeping the max message size small. Otherwise an attacker might send multiple fake packets with 2GB headers, causing the server to run out of memory after allocating multiple large packets.")]
        [FormerlySerializedAs("MaxMessageSize")] public int serverMaxMessageSize = 16 * 1024;

        //Interaction with javascript - this function is defined in Assets/Plugins/plugins.jslib
        #if !UNITY_EDITOR && UNITY_WEBGL
            [DllImport("__Internal")]
            public static extern void webGLSendString (string x);
        #else
            //This state is not allowed, try to kill myself (not sure how to? FIXME)
            public void webGLSendString (string x) {
                Debug.Log("ERROR -> PeerJS can only run inside the browser");
            } 
        #endif

        private bool AmIServer = false;
        private bool clientConnectSuccess = false;


        public override bool Available()
        {
            return false;
        }

        // client
        public override bool ClientConnected() 
        {
            return clientConnectSuccess;
        }
        public override void ClientConnect(string address) 
        {
            webGLSendString("{\"task\":\"clientConnect\",\"address\":\""+address+"\"}");
        }
        public override void ClientSend(int channelId, ArraySegment<byte> segment)
        {
            string msg =  "{\"task\":\"clientSend\",\"data\":\"";
            for ( int i = segment.Offset; i < (segment.Offset + segment.Count); i++ ) {
                msg+=segment.Array[i]+",";
            }
            msg += "\"}";
            webGLSendString(msg);
        }
        public override void ClientDisconnect() 
        {
            if (!AmIServer) webGLSendString("{\"task\":\"clientDisconnect\"}");
        }

        public override Uri ServerUri() {
            Debug.Log("ERROR -> This function doesn't exist in peerJS. Should it exist?");
            UriBuilder builder = new UriBuilder();
            return builder.Uri;
        }

        // server
        public override bool ServerActive() 
        {
            Debug.Log("ERROR -> ServerActive() has not been implemented in PeerJS");
            return false;
        }
        public override void ServerStart() 
        {
            AmIServer = true;
            webGLSendString("{\"task\":\"serverStart\"}");
        } 
        public override void ServerSend(int connectionId, int channelId, ArraySegment<byte> segment)
        {
            string msg =  "{\"task\":\"serverSend\",\"channel\":"+connectionId+",\"data\":\"";
            for ( int i = segment.Offset; i < (segment.Offset + segment.Count); i++ ) {
                msg+=segment.Array[i]+",";
            }
            msg += "\"}";
            webGLSendString(msg);
        }
        public bool ProcessServerMessage()
        {
            Debug.Log("ERROR -> ProcessServerMessage() has not been implemented in PeerJS. Should it exist?");
            return false;
        }
        public override bool ServerDisconnect(int connectionId) 
        {
            Debug.Log("Disconnecting the server");
            webGLSendString("{\"task\":\"serverDisconnect\",\"channel\":"+connectionId+"}");
            return true;
        }
        public override string ServerGetClientAddress(int connectionId)
        {
            Debug.Log("ERROR -> ServerGetClientAddress() has not been implemented in PeerJS. Should it exist?");
            return "";
        }
        public override void ServerStop() 
        {
            Debug.Log("Destroying the server");
            webGLSendString("{\"task\":\"serverDestroy\"}");
            AmIServer = false;
        }

        // common
        public override void Shutdown() 
        {
            Debug.Log("Destroying the server");
            webGLSendString("{\"task\":\"serverDestroy\"}");
            AmIServer = false;
        }

        public override int GetMaxPacketSize(int channelId) 
        {
            return serverMaxMessageSize;
        }

        public void wdkTestFunction() {
            Debug.Log("You hit my test function");
        }
    }
}
