using System;
using System.Text;
using SimpleJson;
using System.Collections;
using System.Collections.Generic;
using MyJson;
namespace Pomelo.Protobuf
{
    public class MsgEncoder
    {
        private JsonNode_Object protos { set; get; }//The message format(like .proto file)
        private Encoder encoder { set; get; }
        private Util util { set; get; }

        public MsgEncoder(JsonNode_Object protos)
        {
            if (protos == null) protos = new JsonNode_Object();

            this.protos = protos;
            this.util = new Util();
        }

        /// <summary>
        /// Encode the message from server.
        /// </summary>
        /// <param name='route'>
        /// Route.
        /// </param>
        /// <param name='msg'>
        /// Message.
        /// </param>
        public byte[] encode(string route, JsonNode_Object msg)
        {
            byte[] returnByte = null;
            IJsonNode proto;
            if (this.protos.TryGetValue(route, out proto))
            {
                if (!checkMsg(msg, (JsonNode_Object)proto))
                {
                    return null;
                }
                int length = Encoder.byteLength(msg.ToString()) * 2;
                int offset = 0;
                byte[] buff = new byte[length];
                offset = encodeMsg(buff, offset, (JsonNode_Object)proto, msg);
                returnByte = new byte[offset];
                for (int i = 0; i < offset; i++)
                {
                    returnByte[i] = buff[i];
                }
            }
            return returnByte;
        }

        /// <summary>
        /// Check the message.
        /// </summary>
        private bool checkMsg(JsonNode_Object msg, JsonNode_Object proto)
        {
            ICollection<string> protoKeys = proto.Keys;
            foreach (string key in protoKeys)
            {
                JsonNode_Object value = (JsonNode_Object)proto[key];
                IJsonNode proto_option;
                if (value.TryGetValue("option", out proto_option))
                {
                    switch (proto_option.ToString())
                    {
                        case "required":
                            if (!msg.ContainsKey(key))
                            {
                                return false;
                            }
                            else
                            {

                            }
                            break;
                        case "optional":
                            IJsonNode value_type;

                            JsonNode_Object messages = (JsonNode_Object)proto["__messages"];

                            value_type = value["type"];

                            if (msg.ContainsKey(key))
                            {
                                IJsonNode value_proto;

                                if (messages.TryGetValue(value_type.ToString(), out value_proto) || protos.TryGetValue("message " + value_type.ToString(), out value_proto))
                                {
                                    checkMsg((JsonNode_Object)msg[key], (JsonNode_Object)value_proto);
                                }
                            }
                            break;
                        case "repeated":
                            IJsonNode msg_name;
                            IJsonNode msg_type;
                            if (value.TryGetValue("type", out value_type) && msg.TryGetValue(key, out msg_name))
                            {
                                if (((JsonNode_Object)proto["__messages"]).TryGetValue(value_type.ToString(), out msg_type) || protos.TryGetValue("message " + value_type.ToString(), out msg_type))
                                {
                                    for (int i=0;i<msg_name.GetListCount();i++)
                                    {
                                        IJsonNode item = msg_name.GetArrayItem(i);
                                        if (!checkMsg((JsonNode_Object)item, (JsonNode_Object)msg_type))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Encode the message.
        /// </summary>
        private int encodeMsg(byte[] buffer, int offset, JsonNode_Object proto, JsonNode_Object msg)
        {
            ICollection<string> msgKeys = msg.Keys;
            foreach (string key in msgKeys)
            {
                IJsonNode value;
                if (proto.TryGetValue(key, out value))
                {
                    IJsonNode value_option;
                    if (((JsonNode_Object)value).TryGetValue("option", out value_option))
                    {
                        switch (value_option.ToString())
                        {
                            case "required":
                            case "optional":
                                IJsonNode value_type, value_tag;
                                if (((JsonNode_Object)value).TryGetValue("type", out value_type) && ((JsonNode_Object)value).TryGetValue("tag", out value_tag))
                                {
                                    offset = this.writeBytes(buffer, offset, this.encodeTag(value_type.ToString(), Convert.ToInt32(value_tag.AsInt())));
                                    offset = this.encodeProp(msg[key], value_type.ToString(), offset, buffer, proto);
                                }
                                break;
                            case "repeated":
                                IJsonNode msg_key;
                                if (msg.TryGetValue(key, out msg_key))
                                {
                                    if (msg_key.AsList().Count > 0)
                                    {
                                        offset = encodeArray(msg_key.AsList(), (JsonNode_Object)value, offset, buffer, proto);
                                    }
                                }
                                break;
                        }
                    }

                }
            }
            return offset;
        }

        /// <summary>
        /// Encode the array type.
        /// </summary>
        private int encodeArray(IList<IJsonNode> msg, JsonNode_Object value, int offset, byte[] buffer, JsonNode_Object proto)
        {
            IJsonNode value_type, value_tag;
            if (value.TryGetValue("type", out value_type) && value.TryGetValue("tag", out value_tag))
            {
                if (this.util.isSimpleType(value_type.ToString()))
                {
                    offset = this.writeBytes(buffer, offset, this.encodeTag(value_type.ToString(), Convert.ToInt32(value_tag.AsInt())));
                    offset = this.writeBytes(buffer, offset, Encoder.encodeUInt32((uint)msg.Count));
                    foreach (var item in msg)
                    {
                        offset = this.encodeProp(item, value_type.ToString(), offset, buffer, null);
                    }
                }
                else
                {
                    foreach (var item in msg)
                    {
                        offset = this.writeBytes(buffer, offset, this.encodeTag(value_type.ToString(), Convert.ToInt32(value_tag.AsInt())));
                        offset = this.encodeProp(item, value_type.ToString(), offset, buffer, proto);
                    }
                }
            }
            return offset;
        }

        /// <summary>
        /// Encode each item in message.
        /// </summary>
        private int encodeProp(IJsonNode value, string type, int offset, byte[] buffer, JsonNode_Object proto)
        {
            switch (type)
            {
                case "uInt32":
                    this.writeUInt32(buffer, ref offset, value);
                    break;
                case "int32":
                case "sInt32":
                    this.writeInt32(buffer, ref offset, value);
                    break;
                case "float":
                    this.writeFloat(buffer, ref offset, value);
                    break;
                case "double":
                    this.writeDouble(buffer, ref offset, value);
                    break;
                case "string":
                    this.writeString(buffer, ref offset, value);
                    break;
                default:
                    IJsonNode __messages;
                    IJsonNode __message_type;

                    if (proto.TryGetValue("__messages", out __messages))
                    {
                        if (((JsonNode_Object)__messages).TryGetValue(type, out __message_type) || protos.TryGetValue("message " + type, out __message_type))
                        {
                            byte[] tembuff = new byte[Encoder.byteLength(value.ToString()) * 3];
                            int length = 0;
                            length = this.encodeMsg(tembuff, length, (JsonNode_Object)__message_type, (JsonNode_Object)value);
                            offset = writeBytes(buffer, offset, Encoder.encodeUInt32((uint)length));
                            for (int i = 0; i < length; i++)
                            {
                                buffer[offset] = tembuff[i];
                                offset++;
                            }
                        }
                    }
                    break;
            }
            return offset;
        }

        //Encode string.
        private void writeString(byte[] buffer, ref int offset, IJsonNode value)
        {
            int le = Encoding.UTF8.GetByteCount(value.AsString());
            offset = writeBytes(buffer, offset, Encoder.encodeUInt32((uint)le));
            byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
            this.writeBytes(buffer, offset, bytes);
            offset += le;
        }

        //Encode double.
        private void writeDouble(byte[] buffer, ref int offset, IJsonNode value)
        {
            WriteRawLittleEndian64(buffer, offset, (ulong)BitConverter.DoubleToInt64Bits(value.AsDouble()));
            offset += 8;
        }

        //Encode float.
        private void writeFloat(byte[] buffer, ref int offset, IJsonNode value)
        {
            this.writeBytes(buffer, offset, Encoder.encodeFloat((float)value.AsDouble()));
            offset += 4;
        }

        ////Encode UInt32.
        private void writeUInt32(byte[] buffer, ref int offset, IJsonNode value)
        {
            offset = writeBytes(buffer, offset, Encoder.encodeUInt32((uint)value.AsInt()));
        }

        //Encode Int32
        private void writeInt32(byte[] buffer, ref int offset, IJsonNode value)
        {
            // offset = writeBytes(buffer, offset, Encoder.encodeSInt32(value.ToString()));
            offset = writeBytes(buffer, offset, Encoder.encodeSInt32(value.AsInt()));
        }

        //Write bytes to buffer.
        private int writeBytes(byte[] buffer, int offset, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                buffer[offset] = bytes[i];
                offset++;
            }
            return offset;
        }

        //Encode tag.
        private byte[] encodeTag(string type, int tag)
        {
            int flag = this.util.containType(type);
            return Encoder.encodeUInt32((uint)(tag << 3 | flag));
        }


        private void WriteRawLittleEndian64(byte[] buffer, int offset, ulong value)
        {
            buffer[offset++] = ((byte)value);
            buffer[offset++] = ((byte)(value >> 8));
            buffer[offset++] = ((byte)(value >> 16));
            buffer[offset++] = ((byte)(value >> 24));
            buffer[offset++] = ((byte)(value >> 32));
            buffer[offset++] = ((byte)(value >> 40));
            buffer[offset++] = ((byte)(value >> 48));
            buffer[offset++] = ((byte)(value >> 56));
        }
    }
}