﻿using Protocol;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using LockStepDemo.LockStepFrameWork;

public class SyncSession : AppSession<SyncSession, ProtocolRequestBase>
{
    public Player player;
    public ConnectionComponent m_connect;

    protected override void OnSessionStarted()
    {
        base.OnSessionStarted();
        Debug.Log("OnSessionStarted ");
#if KCP
        if (s_protocolInfo == null)
        {
            s_protocolInfo = ReadProtocolInfo(FileTool.ReadStringByFile(Environment.CurrentDirectory + "/Network/" + c_ProtocolFileName + ".txt"));
            ReadMethodNameInfo(
                out s_methodNameInfo,
                out s_methodIndexInfo,
                FileTool.ReadStringByFile(Environment.CurrentDirectory + "/Network/" + c_methodNameInfoFileName + ".txt"));
        }

        init_kcp((UInt32)new Random((int)DateTime.Now.Ticks).Next(1, Int32.MaxValue));
        KCPUpdate.GetInstance().Add(Update);
#endif
    }

    protected override void HandleUnknownRequest(ProtocolRequestBase requestInfo)
    {
#if !KCP
        try
        {
            //解析并派发
            ProtocolAnalysisService.AnalysisAndDispatchMessage(this, requestInfo);
           
        }
        catch (Exception e)
        {
            Debug.LogError("AnalysisAndDispatchMessage :" + requestInfo.Key + "\nException: " + e.ToString());
        }
#endif
    }

    protected override void OnSessionClosed(CloseReason reason)
    {
        #if KCP
        KCPUpdate.GetInstance().Remove(Update);
        #endif
        base.OnSessionClosed(reason);
    }

    #region KCP
#if KCP
    public KCP mKcp;
    //public EntityBase m_entity;

    private bool mNeedUpdateFlag;
    private UInt32 mNextUpdateTime;
    private SwitchQueue<byte[]> mRecvQueue = new SwitchQueue<byte[]>(128);

    private static readonly DateTime utc_time = new DateTime(1970, 1, 1);

    public const int TYPE_string = 1;
    public const int TYPE_int32 = 2;
    public const int TYPE_double = 3;
    public const int TYPE_bool = 4;
    public const int TYPE_custom = 5;

    public const int TYPE_int8 = 6;
    public const int TYPE_int16 = 7;
    public const int RT_repeated = 1;
    public const int RT_equired = 0;

    public const string c_ProtocolFileName = "ProtocolInfo";
    public const string c_methodNameInfoFileName = "MethodInfo";

    static Dictionary<string, List<Dictionary<string, object>>> s_protocolInfo;

    static Dictionary<int, string> s_methodNameInfo;
    static Dictionary<string, int> s_methodIndexInfo;

    public static UInt32 iclock()
    {
        return (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(utc_time).TotalMilliseconds) & 0xffffffff);
    }

    void init_kcp(UInt32 conv)
    {
        conv = 1;
        mKcp = new KCP(conv, (byte[] buf, int size) =>
        {
            base.Send(buf, 0, buf.Length);
        });

        // fast mode.
        mKcp.NoDelay(1, 10, 2, 1);
        mKcp.WndSize(128, 128);
    }

    public void Send(byte[] buf)
    {
        mKcp.Send(buf);
        mNeedUpdateFlag = true;
    }

    void process_recv_queue()
    {
        mRecvQueue.Switch();

        while (!mRecvQueue.Empty())
        {
            var buf = mRecvQueue.Pop();

            mKcp.Input(buf);
            mNeedUpdateFlag = true;

            for (var size = mKcp.PeekSize(); size > 0; size = mKcp.PeekSize())
            {
                var buffer = new byte[size];
                if (mKcp.Recv(buffer) > 0)
                {
                    //evHandler(buffer);
                    //这里开始解包

                    try
                    {
                        //解析并派发
                        ByteArray ba = new ByteArray();
                        ba.WriteALLBytes(buffer);
                        ProtocolRequestBase msg = Analysis(ba);

                        ProtocolAnalysisService.AnalysisAndDispatchMessage(this, msg);

                    }
                    catch (Exception e)
                    {
                        Debug.LogError("AnalysisAndDispatchMessage :\nException: " + e.ToString());
                    }
                }
            }
        }
    }

    public void Update()
    {
        update(iclock());
    }

    void update(UInt32 current)
    {
        process_recv_queue();

        if (mNeedUpdateFlag || current >= mNextUpdateTime)
        {
            mKcp.Update(current);
            mNextUpdateTime = mKcp.Check(current);
            mNeedUpdateFlag = false;
        }
    }

    public void pushData(byte[] data)
    {
        // push udp packet to switch queue.
        mRecvQueue.Push(data);
        //临时用用
        //Update();
    }

    #region 解包

    ProtocolRequestBase Analysis(ByteArray bytes)
    {
        ProtocolRequestBase msg = new ProtocolRequestBase();
        //Debug.Log("ReceiveDataLoad : " + BitConverter.ToString(bytes));
        bytes.ReadUShort(); //消息长度
        bytes.ReadByte();  //模块名

        int methodIndex = bytes.ReadUShort(); //方法名

        try
        {
            msg.Key = s_methodNameInfo[methodIndex];
            int re_len = bytes.Length - 5;
            msg.m_data = AnalysisData(msg.Key, bytes.ReadBytes(re_len));
        }
        catch (Exception e)
        {
            Debug.LogError("methodIndex:" + methodIndex + " \nEx:" + e.ToString());
        }

        return msg;
    }

    Dictionary<string, object> AnalysisData(string MessageType, byte[] bytes)
    {
        //Debug.Log("MessageType:" + MessageType + "AnalysisData: " +  BitConverter.ToString(bytes));

        string fieldName = "";
        string customType = "";
        int fieldType = 0;
        int repeatType = 0;
        try
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            ByteArray ba = new ByteArray();

            ba.clear();
            ba.Add(bytes);

            string messageTypeTemp = "m_" + MessageType + "_s";
            if (!s_protocolInfo.ContainsKey(messageTypeTemp))
            {
                throw new Exception("ProtocolInfo NOT Exist ->" + messageTypeTemp + "<-");
            }

            List<Dictionary<string, object>> tableInfo = s_protocolInfo["m_" + MessageType + "_s"];

            for (int i = 0; i < tableInfo.Count; i++)
            {
                fieldType = (int)tableInfo[i]["type"];
                repeatType = (int)tableInfo[i]["spl"];
                fieldName = (string)tableInfo[i]["name"];

                if (fieldType == TYPE_string)
                {
                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadStringList(ba);
                    }
                    else
                    {
                        data[fieldName] = ReadString(ba);
                    }
                }
                else if (fieldType == TYPE_bool)
                {
                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadBoolList(ba);
                    }
                    else
                    {
                        data[fieldName] = ReadBool(ba);
                    }
                }
                else if (fieldType == TYPE_double)
                {
                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadDoubleList(ba);
                    }
                    else
                    {
                        data[fieldName] = ReadDouble(ba);
                    }
                }
                else if (fieldType == TYPE_int32)
                {
                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadIntList(ba);
                    }
                    else
                    {
                        data[fieldName] = ReadInt32(ba);
                    }
                }
                else if (fieldType == TYPE_int16)
                {
                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadShortList(ba);
                    }
                    else
                    {
                        data[fieldName] = ReadInt16(ba);
                    }
                }
                else if (fieldType == TYPE_int8)
                {
                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadInt8List(ba);
                    }
                    else
                    {
                        data[fieldName] = ReadInt8(ba);
                    }
                }
                else
                {
                    customType = (string)tableInfo[i]["vp"];

                    if (repeatType == RT_repeated)
                    {
                        data[fieldName] = ReadDictionaryList(customType, ba);
                    }
                    else
                    {
                        data[fieldName] = ReadDictionary(customType, ba);
                    }
                }
            }

            return data;
        }
        catch (Exception e)
        {
            throw new Exception(@"AnalysisData Excepiton Data is ->" + MessageType
                        + "<-\nFieldName:->" + fieldName
                        + "<-\nFieldType:->" + GetFieldType(fieldType)
                        + "<-\nRepeatType:->" + GetRepeatType(repeatType)
                        + "<-\nCustomType:->" + customType
                        + "<-\n" + e.ToString());
        }
    }

    private string ReadString(ByteArray ba)
    {
        uint len = (uint)ba.ReadUShort();
        return ba.ReadUTFBytes(len);
    }
    private List<string> ReadStringList(ByteArray ba)
    {
        List<string> tbl = new List<string>();
        tbl.Clear();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            tbl.Add(ReadString(ba));
        }
        return tbl;
    }
    private bool ReadBool(ByteArray ba)
    {
        return ba.ReadBoolean();
    }
    private List<bool> ReadBoolList(ByteArray ba)
    {
        List<bool> tbl = new List<bool>();
        tbl.Clear();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            tbl.Add(ReadBool(ba));
        }
        return tbl;
    }
    private int ReadInt(ByteArray ba)
    {
        return ba.ReadUInt();
    }

    private int ReadInt32(ByteArray ba)
    {
        return ba.ReadInt32();
    }

    private int ReadInt16(ByteArray ba)
    {
        return ba.ReadInt16();
    }
    private int ReadInt8(ByteArray ba)
    {
        return ba.ReadInt8();
    }
    private List<int> ReadIntList(ByteArray ba)
    {
        List<int> tbl = new List<int>();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            int tem_o_read_int = ReadInt32(ba);
            tbl.Add(tem_o_read_int);
        }

        return tbl;
    }

    private List<int> ReadShortList(ByteArray ba)
    {
        List<int> tbl = new List<int>();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            int tem_o_read_int = ReadInt16(ba);
            tbl.Add(tem_o_read_int);
        }

        return tbl;
    }

    private List<int> ReadInt8List(ByteArray ba)
    {
        List<int> tbl = new List<int>();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            int tem_o_read_int = ReadInt8(ba);
            tbl.Add(tem_o_read_int);
        }

        return tbl;
    }

    private double ReadDouble(ByteArray ba)
    {
        double tem_double = ba.ReadDouble();
        return Math.Floor(tem_double * 1000) / 1000;
    }
    private List<double> ReadDoubleList(ByteArray ba)
    {
        List<double> tbl = new List<double>();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            tbl.Add(ReadDouble(ba));
        }
        return tbl;
    }

    private Dictionary<string, object> ReadDictionary(string dictName, ByteArray ba)
    {
        int fieldType = 0;
        int repeatType = 0;
        string fieldName = null;
        string customType = null;

        try
        {
            int st_len = ba.ReadUInt();

            Dictionary<string, object> tbl = new Dictionary<string, object>();

            if (st_len == 0)
            {
                return tbl;
            }

            List<Dictionary<string, object>> tableInfo = s_protocolInfo[dictName];

            for (int i = 0; i < tableInfo.Count; i++)
            {
                fieldType = (int)tableInfo[i]["type"];
                repeatType = (int)tableInfo[i]["spl"];
                fieldName = (string)tableInfo[i]["name"];

                if (fieldType == TYPE_string)
                {
                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadStringList(ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadString(ba);
                    }
                }
                else if (fieldType == TYPE_bool)
                {
                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadBoolList(ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadBool(ba);
                    }
                }
                else if (fieldType == TYPE_double)
                {
                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadDoubleList(ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadDouble(ba);
                    }
                }
                else if (fieldType == TYPE_int32)
                {
                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadIntList(ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadInt32(ba);
                    }
                }
                else if (fieldType == TYPE_int16)
                {
                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadShortList(ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadInt16(ba);
                    }
                }
                else if (fieldType == TYPE_int8)
                {
                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadInt8List(ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadInt8(ba);
                    }
                }
                else
                {
                    customType = (string)tableInfo[i]["vp"];

                    if (repeatType == RT_repeated)
                    {
                        tbl[fieldName] = ReadDictionaryList(customType, ba);
                    }
                    else
                    {
                        tbl[fieldName] = ReadDictionary(customType, ba);
                    }
                }
            }
            return tbl;

        }
        catch (Exception e)
        {
            throw new Exception(@"ReadDictionary Excepiton DictName is ->" + dictName
                        + "<-\nFieldName:->" + fieldName
                        + "<-\nFieldType:->" + GetFieldType(fieldType)
                        + "<-\nRepeatType:->" + GetRepeatType(repeatType)
                        + "<-\nCustomType:->" + customType
                        + "<-\n" + e.ToString());
        }
    }

    private List<Dictionary<string, object>> ReadDictionaryList(string str, ByteArray ba)
    {
        List<Dictionary<string, object>> stbl = new List<Dictionary<string, object>>();

        int len1 = ba.ReadUShort();
        ba.ReadUInt();

        for (int i = 0; i < len1; i++)
        {
            stbl.Add(ReadDictionary(str, ba));
        }
        return stbl;
    }
    #endregion

    #region 发包

    List<byte> GetSendByte(string messageType, Dictionary<string, object> data)
    {
        try
        {
            string messageTypeTemp = "m_" + messageType + "_s";
            if (!s_protocolInfo.ContainsKey(messageTypeTemp))
            {
                throw new Exception("ProtocolInfo NOT Exist ->" + messageTypeTemp + "<-");
            }

            return GetCustomTypeByte(messageTypeTemp, data);
        }
        catch (Exception e)
        {
            throw new Exception(@"ProtocolService GetSendByte Excepiton messageType is ->" + messageType
                + "<-\n" + e.ToString());
        }
    }

    int GetStringListLength(List<object> list)
    {
        int len = 0;
        for (int i = 0; i < list.Count; i++)
        {
            byte[] bs = Encoding.UTF8.GetBytes((string)list[i]);
            len = len + bs.Length;

        }
        return len;
    }

    List<List<byte>> m_arrayCache = new List<List<byte>>();
    int GetCustomListLength(string customType, List<object> list)
    {
        m_arrayCache.Clear();
        int len = 0;
        for (int i = 0; i < list.Count; i++)
        {
            List<byte> bs = GetCustomTypeByte(customType, (Dictionary<string, object>)list[i]);
            m_arrayCache.Add(bs);
            len = len + bs.Count + 4;
        }
        return len;
    }

    private List<byte> GetCustomTypeByte(string customType, Dictionary<string, object> data)
    {
        string fieldName = null;
        int fieldType = 0;
        int repeatType = 0;

        try
        {
            ByteArray Bytes = new ByteArray();
            //ByteArray Bytes = new ByteArray();
            Bytes.clear();

            if (!s_protocolInfo.ContainsKey(customType))
            {
                throw new Exception("ProtocolInfo NOT Exist ->" + customType + "<-");
            }

            List<Dictionary<string, object>> tableInfo = s_protocolInfo[customType];

            for (int i = 0; i < tableInfo.Count; i++)
            {
                Dictionary<string, object> currentField = tableInfo[i];
                fieldType = (int)currentField["type"];
                fieldName = (string)currentField["name"];
                repeatType = (int)currentField["spl"];

                if (fieldType == TYPE_string)
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            Bytes.WriteString((string)data[fieldName]);
                        }
                        else
                        {
                            List<object> list = (List<object>)data[fieldName];

                            Bytes.WriteShort(list.Count);
                            Bytes.WriteInt(GetStringListLength(list));
                            for (int i2 = 0; i2 < list.Count; i2++)
                            {
                                Bytes.WriteString((string)list[i2]);
                            }
                        }
                    }
                    else
                    {
                        Bytes.WriteShort(0);
                    }
                }
                else if (fieldType == TYPE_bool)
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            Bytes.WriteBoolean((bool)data[fieldName]);
                        }
                        else
                        {
                            List<object> tb = (List<object>)data[fieldName];
                            Bytes.WriteShort(tb.Count);
                            Bytes.WriteInt(tb.Count);
                            for (int i2 = 0; i2 < tb.Count; i2++)
                            {
                                Bytes.WriteBoolean((bool)tb[i2]);
                            }
                        }
                    }
                }
                else if (fieldType == TYPE_double)
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            Bytes.WriteDouble((float)data[fieldName]);
                        }
                        else
                        {
                            List<object> tb = (List<object>)data[fieldName];
                            Bytes.WriteShort(tb.Count);
                            Bytes.WriteInt(tb.Count * 8);
                            for (int j = 0; j < tb.Count; j++)
                            {
                                Bytes.WriteDouble((float)tb[j]);
                            }
                        }
                    }
                }
                else if (fieldType == TYPE_int32)
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            Bytes.WriteInt(int.Parse(data[fieldName].ToString()));
                        }
                        else
                        {
                            List<object> tb = (List<object>)data[fieldName];
                            Bytes.WriteShort(tb.Count);
                            Bytes.WriteInt(tb.Count * 4);
                            for (int i2 = 0; i2 < tb.Count; i2++)
                            {
                                Bytes.WriteInt(int.Parse(tb[i2].ToString()));
                            }
                        }
                    }
                }
                else if (fieldType == TYPE_int16)
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            Bytes.WriteShort(int.Parse(data[fieldName].ToString()));
                        }
                        else
                        {
                            List<object> tb = (List<object>)data[fieldName];
                            Bytes.WriteShort(tb.Count);
                            Bytes.WriteInt(tb.Count * 2);
                            for (int i2 = 0; i2 < tb.Count; i2++)
                            {
                                Bytes.WriteShort(int.Parse(tb[i2].ToString()));
                            }
                        }
                    }
                }
                else if (fieldType == TYPE_int8)
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            Bytes.WriteInt8(int.Parse(data[fieldName].ToString()));
                        }
                        else
                        {
                            List<object> tb = (List<object>)data[fieldName];
                            Bytes.WriteShort(tb.Count);
                            Bytes.WriteInt(tb.Count);
                            for (int i2 = 0; i2 < tb.Count; i2++)
                            {
                                Bytes.WriteInt8(int.Parse(tb[i2].ToString()));
                            }
                        }
                    }
                }
                else
                {
                    if (data.ContainsKey(fieldName))
                    {
                        if (repeatType == RT_equired)
                        {
                            customType = (string)currentField["vp"];
                            Bytes.bytes.AddRange(GetSendByte(customType, (Dictionary<string, object>)data[fieldName]));
                        }
                        else
                        {
                            List<object> tb = (List<object>)data[fieldName];

                            Bytes.WriteShort(tb.Count);
                            //这里会修改m_arrayCatch的值，下面就可以直接使用
                            Bytes.WriteInt(GetCustomListLength(customType, tb));

                            for (int j = 0; j < m_arrayCache.Count; j++)
                            {
                                List<byte> tempb = m_arrayCache[j];
                                Bytes.WriteInt(tempb.Count);
                                Bytes.bytes.AddRange(tempb);
                            }
                        }
                    }
                }
            }
            return Bytes.bytes;
        }
        catch (Exception e)
        {
            throw new Exception(@"GetCustomTypeByte Excepiton CustomType is ->" + customType
               + "<-\nFieldName:->" + fieldName
               + "<-\nFieldType:->" + GetFieldType(fieldType)
               + "<-\nRepeatType:->" + GetRepeatType(repeatType)
               + "<-\nCustomType:->" + customType
               + "<-\n" + e.ToString());
        }
    }

    string GetFieldType(int fieldType)
    {
        switch (fieldType)
        {
            case TYPE_string: return "TYPE_string";
            case TYPE_int32: return "TYPE_int32";
            case TYPE_int16: return "TYPE_int16";
            case TYPE_int8: return "TYPE_int8";
            case TYPE_double: return "TYPE_double";
            case TYPE_bool: return "TYPE_bool";
            case TYPE_custom: return "TYPE_custom";
            default: return "Error";
        }
    }

    string GetRepeatType(int repeatType)
    {
        switch (repeatType)
        {
            case RT_repeated: return "RT_repeated";
            case RT_equired: return "RT_equired";
            default: return "Error";
        }
    }




    #region 读取protocol信息

    public static Dictionary<string, List<Dictionary<string, object>>> ReadProtocolInfo(string content)
    {
        Dictionary<string, List<Dictionary<string, object>>> protocolInfo = new Dictionary<string, List<Dictionary<string, object>>>();

        AnalysisProtocolStatus currentStatus = AnalysisProtocolStatus.None;
        List<Dictionary<string, object>> msgInfo = new List<Dictionary<string, object>>();
        Regex rgx = new Regex(@"^message\s(\w+)");

        string[] lines = content.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string currentLine = lines[i];

            if (currentStatus == AnalysisProtocolStatus.None)
            {
                if (currentLine.Contains("message"))
                {
                    string msgName = rgx.Match(currentLine).Groups[1].Value;

                    msgInfo = new List<Dictionary<string, object>>();

                    if (protocolInfo.ContainsKey(msgName))
                    {
                        //Logger.Debug("protocol 有重复的Key! :" + msgName);
                        //Debug.LogError("protocol 有重复的Key! :" + msgName);
                    }
                    else
                    {
                        protocolInfo.Add(msgName, msgInfo);
                    }

                    currentStatus = AnalysisProtocolStatus.Message;
                }
            }
            else
            {
                if (currentLine.Contains("}"))
                {
                    currentStatus = AnalysisProtocolStatus.None;
                    msgInfo = null;
                }
                else
                {
                    if (currentLine.Contains("required"))
                    {
                        Dictionary<string, object> currentFeidInfo = new Dictionary<string, object>();

                        currentFeidInfo.Add("spl", RT_equired);

                        AddName(currentLine, currentFeidInfo);
                        AddType(currentLine, currentFeidInfo);

                        msgInfo.Add(currentFeidInfo);
                    }
                    else if (currentLine.Contains("repeated"))
                    {
                        Dictionary<string, object> currentFeidInfo = new Dictionary<string, object>();

                        currentFeidInfo.Add("spl", RT_repeated);

                        AddName(currentLine, currentFeidInfo);
                        AddType(currentLine, currentFeidInfo);

                        msgInfo.Add(currentFeidInfo);
                    }
                }
            }
        }

        return protocolInfo;
    }


    static Regex m_TypeRgx = new Regex(@"^\s+\w+\s+(\w+)\s+\w+");

    static void AddType(string currentLine, Dictionary<string, object> currentFeidInfo)
    {
        if (currentLine.Contains("int32"))
        {
            currentFeidInfo.Add("type", TYPE_int32);
        }
        else if (currentLine.Contains("int16"))
        {
            currentFeidInfo.Add("type", TYPE_int16);
        }
        else if (currentLine.Contains("int8"))
        {
            currentFeidInfo.Add("type", TYPE_int8);
        }
        else if (currentLine.Contains("string"))
        {
            currentFeidInfo.Add("type", TYPE_string);
        }
        else if (currentLine.Contains("double"))
        {
            currentFeidInfo.Add("type", TYPE_double);
        }
        else if (currentLine.Contains("bool"))
        {
            currentFeidInfo.Add("type", TYPE_bool);
        }
        else
        {
            currentFeidInfo.Add("type", TYPE_custom);
            currentFeidInfo.Add("vp", m_TypeRgx.Match(currentLine).Groups[1].Value);
        }
    }

    static Regex m_NameRgx = new Regex(@"^\s+\w+\s+\w+\s+(\w+)");

    static void AddName(string currentLine, Dictionary<string, object> currentFeidInfo)
    {
        currentFeidInfo.Add("name", m_NameRgx.Match(currentLine).Groups[1].Value);
    }

    enum AnalysisProtocolStatus
    {
        None,
        Message
    }

    #endregion
    #region 读取消息号映射

    public static void ReadMethodNameInfo(out Dictionary<int, string> methodNameInfo, out Dictionary<string, int> methodIndexInfo, string content)
    {
        methodNameInfo = new Dictionary<int, string>();
        methodIndexInfo = new Dictionary<string, int>();

        Regex rgx = new Regex(@"^(\d+),(\w+)");

        string[] lines = content.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(","))
            {
                var res = rgx.Match(lines[i]);

                string index = res.Groups[1].Value;
                string indexName = res.Groups[2].Value;

                methodNameInfo.Add(int.Parse(index), indexName);
                methodIndexInfo.Add(indexName, int.Parse(index));
            }
        }
    }

    int GetMethodIndex(string messageType)
    {
        try
        {
            return s_methodIndexInfo[messageType];
        }
        catch
        {
            throw new Exception("GetMethodIndex ERROR! NOT Find " + messageType);
        }
    }

    #endregion
    #endregion

#endif
    #endregion
}
