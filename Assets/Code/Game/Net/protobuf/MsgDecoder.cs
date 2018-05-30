using System;
using System.Text;
using SimpleJson;
using System.Collections;
using System.Collections.Generic;
using MyJson;
namespace Pomelo.Protobuf
{
    public class MsgDecoder
    {
        private JsonNode_Object protos { set; get; }//The message format(like .proto file)
        private int offset { set; get; }
        private byte[] buffer { set; get; }//The binary message from server.
        private Util util { set; get; }

        public MsgDecoder(JsonNode_Object protos)
        {
            if (protos == null) protos = new JsonNode_Object();

            this.protos = protos;
            this.util = new Util();
        }

        /// <summary>
        /// Decode message from server.
        /// </summary>
        /// <param name='route'>
        /// Route.
        /// </param>
        /// <param name='buf'>
        /// JsonNode_Object.
        /// </param>
        public JsonNode_Object decode(string route, byte[] buf)
        {
            this.buffer = buf;
            this.offset = 0;
            IJsonNode proto = null;
            if (this.protos.TryGetValue(route, out proto))
            {
                JsonNode_Object msg = new JsonNode_Object();
                return this.decodeMsg(msg, (JsonNode_Object)proto, this.buffer.Length);
            }
            return null;
        }


        /// <summary>
        /// Decode the message.
        /// </summary>
        /// <returns>
        /// The message.
        /// </returns>
        /// <param name='msg'>
        /// JsonNode_Object.
        /// </param>
        /// <param name='proto'>
        /// JsonNode_Object.
        /// </param>
        /// <param name='length'>
        /// int.
        /// </param>
        private JsonNode_Object decodeMsg(JsonNode_Object msg, JsonNode_Object proto, int length)
        {
            while (this.offset < length)
            {
                Dictionary<string, int> head = this.getHead();
                int tag;
                if (head.TryGetValue("tag", out tag))
                {
                    IJsonNode _tags = null;
                    if (proto.TryGetValue("__tags", out _tags))
                    {
                        IJsonNode name;
                        if (((JsonNode_Object)_tags).TryGetValue(tag.ToString(), out name))
                        {
                            IJsonNode value;
                            if (proto.TryGetValue(name.ToString(), out value))
                            {
                                IJsonNode option;
                                if (((JsonNode_Object)(value)).TryGetValue("option", out option))
                                {
                                    switch (option.ToString())
                                    {
                                        case "optional":
                                        case "required":
                                            IJsonNode type;
                                            if (((JsonNode_Object)(value)).TryGetValue("type", out type))
                                            {
                                                msg[name.ToString()] = this.decodeProp(type.ToString(), proto);
                                            }
                                            break;
                                        case "repeated":
                                            IJsonNode _name;
                                            if (!msg.TryGetValue(name.ToString(), out _name))
                                            {
                                                msg[name.ToString()] =  new JsonNode_Array();
                                            }
                                            IJsonNode value_type;
                                            if (msg.TryGetValue(name.ToString(), out _name) && ((JsonNode_Object)(value)).TryGetValue("type", out value_type))
                                            {
                                                decodeArray((JsonNode_Array)_name, value_type.ToString(), proto);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// Decode array in message.
        /// </summary>
        private void decodeArray(JsonNode_Array list, string type, JsonNode_Object proto)
        {
            if (this.util.isSimpleType(type))
            {
                int length = (int)decodeUInt32();
                for (int i = 0; i < length; i++)
                {
                    list.Add(this.decodeProp(type, null));
                }
            }
            else
            {
                list.Add(this.decodeProp(type, proto));
            }
        }

        /// <summary>
        /// Decode each simple type in message.
        /// </summary>
        private IJsonNode decodeProp(string type, JsonNode_Object proto)
        {
            switch (type)
            {
                case "uInt32":
                    return new JsonNode_ValueNumber( decodeUInt32());
                case "int32":
                case "sInt32":
                    return new JsonNode_ValueNumber( decodeSInt32());
                case "float":
                    return new JsonNode_ValueNumber( this.decodeFloat());
                case "double":
                    return new JsonNode_ValueNumber( this.decodeDouble());
                case "string":
                    return new JsonNode_ValueString( this.decodeString());
                default:
                    return this.decodeObject(type, proto);
            }
        }

        //Decode the user-defined object type in message.
        private JsonNode_Object decodeObject(string type, JsonNode_Object proto)
        {
            if (proto != null)
            {
                IJsonNode __messages;
                if (proto.TryGetValue("__messages", out __messages))
                {
                    IJsonNode _type;
                    if (((JsonNode_Object)__messages).TryGetValue(type, out _type) || protos.TryGetValue("message " + type, out _type))
                    {
                        int l = (int)decodeUInt32();
                        JsonNode_Object msg = new JsonNode_Object();
                        return this.decodeMsg(msg, (JsonNode_Object)_type, this.offset + l);
                    }
                }
            }
            return new JsonNode_Object();
        }

        //Decode string type.
        private string decodeString()
        {
            int length = (int)decodeUInt32();
            string msg_string = Encoding.UTF8.GetString(this.buffer, this.offset, length);
            this.offset += length;
            return msg_string;
        }

        //Decode double type.
        private double decodeDouble()
        {
            double msg_double = BitConverter.Int64BitsToDouble((long)this.ReadRawLittleEndian64());
            this.offset += 8;
            return msg_double;
        }

        //Decode float type
        private float decodeFloat()
        {
            float msg_float = BitConverter.ToSingle(this.buffer, this.offset);
            this.offset += 4;
            return msg_float;
        }

        //Read long in littleEndian
        private ulong ReadRawLittleEndian64()
        {
            ulong b1 = buffer[this.offset];
            ulong b2 = buffer[this.offset + 1];
            ulong b3 = buffer[this.offset + 2];
            ulong b4 = buffer[this.offset + 3];
            ulong b5 = buffer[this.offset + 4];
            ulong b6 = buffer[this.offset + 5];
            ulong b7 = buffer[this.offset + 6];
            ulong b8 = buffer[this.offset + 7];
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
                  | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }

        //Get the type and tag.
        private Dictionary<string, int> getHead()
        {
            int tag = (int)decodeUInt32();
            Dictionary<string, int> head = new Dictionary<string, int>();
            head.Add("type", tag & 0x7);
            head.Add("tag", tag >> 3);
            return head;
        }

        private uint decodeUInt32()
        {
            int length;
            uint ret = Decoder.decodeUInt32(this.offset, this.buffer, out length);
            this.offset += length;
            return ret;
        }

        private int decodeSInt32()
        {
            int length;
            int ret = Decoder.decodeSInt32(this.offset, this.buffer, out length);
            this.offset += length;
            return ret;
        }

        //Get bytes.
        private byte[] getBytes()
        {
            List<byte> arrayList = new List<byte>();
            int pos = this.offset;
            byte b;
            do
            {
                b = this.buffer[pos];
                arrayList.Add(b);
                pos++;
            } while (b >= 128);
            this.offset = pos;
            int length = arrayList.Count;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = arrayList[i];
            }
            return bytes;
        }
    }
}