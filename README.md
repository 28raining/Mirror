## PeerJS branch of Mirror
A WebGL friendly transport layer

## Why?
Allows games to be hosted from inside the browser. None of the other transport layers support this

## How?
It uses peerJS which is a wrapper around WebRTC - the peer to peer connection protocol. This is very similar to Unity's wrapper around WebRTC, but Unity's has no timeline for supporting WebGL.

## Instructions
It is the same as other transport layers, i.e. simply add the peerJS component to the NetworkManager and drag it into the transport box
1. Add "Peer JS Trasnport" as a compnent to Network Manager
2. Drag peerJS to transport box in Network Manager
3. Copy Mirror/Runtime/Transport/PeerJS/WebGLTemplates to Assets/. peerjs should now be selected in build -> player settings -> player -> WebGL Template 
4. (optional for cleanliness) move contents of Mirror/Runtime/Transport/PeerJS/Plugins to Assets/Plugins
5. Build and run!

## Test Coverage
- Pong (simplest game!)
- Tanks (N players)
- Only tested on Chrome

## WebRTC considerations
- To connect two users you need a signalling server, which is basically a DNS server, converting some alias to an IP address. This has very light computation overhead, therefore peerJS provide it for free. You don't need to do anything.
- Because IPv4 ran out of IP addresses, not everyone has a unique IP. You need to use a STUN server to find their IP. Google provide a free STUN server, which peerJS uses by default.
- WebRTC can also be blocked by symmetric NAT's and firewalls. You can use a TURN server to relay WebRTC traffic and this somehow bypasses the afermentioned problems. But... having a server undermines WebRTC! PeerJS supports TURN servers so this could easily be implemented if somebody wanted. Note that WebRTC + TURN server is pretty good because only ~10% of users need a TURN, so 90% of your traffic won't go thru the server (so will be cheaper to run). I've used localhost, Xfinity internet, T-mobile LTE and from inside Apple's network without needing a TURN. Migration to IPv6 means fewer people need TURN servers.

## Known Issues
- Frequently the client has to click twice to connect to the server.
   - Almost certainly is a peerJS signalling server-load issue. Several others mentioned it in GitHub issues. Can launch signalling server in free tier of GCP?
- Doesn't detect when user closes the browser. idk why WebRTC doesn't have a trigger for this. Common solutions disconnect after 5 seconds of no contact
- Cannot run from the game view. PeerJS runs inside the browser, which doesn't exist in the game view.
  - Workaround -> Use KCP in game view, use PeerJS on your builds
- Can't disable logging. Can't figure out location of 'Log.Info/.Error' function that KCP uses
- As of January 2021 there was no investigation into performance
   - Note Unity <-> JS is via JSON. Peer<->Peer is bytes
   - I have no idea what the max number of players is. peerJS signalling server limits to 50 connections. There may be other lower limits.
- Unity WebGL doesn't support Copy+Paste. Use this https://github.com/kou-yeung/WebGLInput
- Only tested in Chrome. There may be a cross browser issue due to webRTC support. But browsers change every week and I haven't tested.
- Only works in browsers. Potentially can run peerJS in nodeJS to run outside of browser. Or wait for Unity WebRTC to work.

## Work To Do
- Add more type checking / error handling in JavaScript
- Add calls to OnServerError, OnClientError

## Questions about Mirror
- ServerUri has to exist in transport for Unity to build but it isn't needed?
- Server tick rate has no effect on how often ServerSend() is called?