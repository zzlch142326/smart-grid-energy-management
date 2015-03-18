# The Goal #

Suppose the city divided into neighborhoods.
Each neighborhoods is a mesh. The mesh is made up of peers (aka `Buildings`) that are able to exchange messages to contract for energy supplies based on their needs.
Each mesh contains a special peer called `Resolver`. The `Resolver` builds the main mesh by exposing the `Local Resolver Service`; it also exposes a `Remote service` that will be described next.

Each neighborhoods is represented by the `Resolver`.

# P2P #

The `Resolver` has a special role in the mesh: forwarding messages to other Resolvers (neighborhoods) in the city to provide energy for its local peers. This is done using a real P2P system.
When a `Building` needs more Energy it sends a special request to its own `Resolver` and this will provide to forward the message to the next Resolver.
Each `Resolver` has a `NetConfig.xml` file associated that describes its knowledghe of the net. When a `Resolver` connects to a Remote `Resolver` it will update the `NetConfig.xml` file downloading the remote file and merging the contacts list.
The struct of the `NetConfig.xml` file is the following

```
<?xml version="1.0" encoding="utf-8" ?>
<Params>
  <Name>Rick Resolver</Name>
  <ServicePort>8082</ServicePort>
  <RemoteHosts>
    <Host>
      <Name>Remote Resolver</Name>
      <IP>192.168.1.97</IP>
      <Port>8082</Port>
    </Host>
    <Host>
      <Name>Yet Another Remote Resolver</Name>
      <IP>192.168.1.124</IP>
      <Port>8082</Port>
    </Host>
  </RemoteHosts>
</Params>
```

# Energy Broker #

The process to find energy in response to a remote request is demanded to the `EnergyBroker`. This component has the role of look up energy in the local mesh, evaluating the proposal and send an `EndProposalMessage` that reports `true` if energy is available, `false` otherwise.
This process is started in a separated Thread using the statement:

```
_brokerThread = new Thread(new ParameterizedThreadStart(_broker.EnergyLookUp));
_brokerThread.Start(message.enReqMessage);
```

As we can see, the message (`enReqMessage`) is passed as a parameter. The broker replace the `messageHeader` changing the `Sender` attribute to `Resolver` name.

```
message.header.Sender = this._name;
```

The `Resolver` will re-change the `messageHeader` before sending back the energy reply message.
There is a _Session Based System_ used to take trace of the message request/reply mechanism.

# The Session System #

Each Header in a message contains a `MessageID` in the C# `Guid` format:

```
[DataMember]
public Guid MessageID { get; set; }
```

This ID can be a new ID for some messages, and a piggyback ID for other messages. This is the philosophy:
Each NEW Energy Request require a new `MessageId` to create a new `Session`. The other followin messages will report this ID in their Header to indicate that belong to that `Session`.

To do this, each message is created with a `getHeader` method, overrided for this purpose in this two ways:
```
public static StandardMessageHeader getHeader(String receiver, String sender) {...}

public static StandardMessageHeader getHeader(String receiver, String sender, Guid SessionMessageID) {...}
```

the first will create a new `Guid`, the second will copy the `SessionId` in the header.