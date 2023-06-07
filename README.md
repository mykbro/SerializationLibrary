# SerializationLibrary
## Disclaimer
This codebase is made for self-teaching and educational purposes only.
Many features like input validation, object disposed checks, some exception handling, etc... are mostly missing.
As such this codebase cannot be considered production ready.


## What's this ?

This is a simple library consisting of classes that will allow the user to easily read/write whole objects of type T from/to a stream leveraging Serialization.
A socket reader/writer couple of classes is also provided (including a threadsafe concurrent version for the socket writer).


## How does it work ?

The 2 main classes are the abstracts ObjectReader&lt;T&gt; and ObjectWriter&lt;T&gt;.
The derived classes provided are:

* #### BinaryObjectReader/Writer 
	which uses the BinaryFormatter and binary serialization (which is deprecated)
* #### DataContractObjectReader/Writer 
	which uses the DataContractSerializer
* #### CustomObjectReader/Writer 
	which uses a custom serialization for types T that will implement ICustomSerializable&lt;T&gt;

The SocketObjectReader and Writer classes are there to abstract away the details of also including the serialization length (in bytes) in the payload, 
as the receiver needs to know how many bytes it'll have to read. 

The SocketObjectWriter will first send and Int32 (4 bytes) representing the length of the object serialization.

The SocketObjectReader will keep listening for a 4 bytes read. It will then convert these 4 bytes to a payloadLength:Int32 and then will fetch a nr of bytes equal to payloadLength.

It will then deserialize this payload to the object.
There are no multiple derived version for the Socket reader/writer. The serialization used must be chosen using an enum valued field. This choice will decide the internal ObjectReader/Writer implementation.


## How should I use this ?
It's a library... just build it and link the assembly :)




