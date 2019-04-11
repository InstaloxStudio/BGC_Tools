﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightJson;

namespace BGC.Study
{
    public class Protocol : IEnumerable<SessionID>
    {
        private static int nextProtocolID = 1;
        public readonly int id;

        public string name;
        public List<SessionID> sessions;
        public JsonObject envVals;

        //Explicitly created Protocols are added to the Protocol dictionary
        public Protocol()
        {
            id = nextProtocolID++;
            sessions = new List<SessionID>();
            envVals = new JsonObject();

            ProtocolManager.protocolDictionary.Add(id, this);
        }

        //Explicitly created Protocols are added to the Protocol dictionary
        public Protocol(string name)
            : this()
        {
            this.name = name;
        }

        //Deserialized Protocols are not added to the Protocol dictionary
        public Protocol(int id)
        {
            this.id = id;
            if (nextProtocolID <= id)
            {
                nextProtocolID = id + 1;
            }

            //Should be assigned by constructing caller
            envVals = null;
        }

        public int Count => sessions.Count;

        public Session this[int i] => sessions[i].Session;

        public void Add(Session session) => sessions.Add(session);

        public void AddRange(IEnumerable<Session> sessions)
        {
            foreach(Session session in sessions)
            {
                this.sessions.Add(session);
            }
        }

        public static void HardClear()
        {
            nextProtocolID = 1;
        }

        #region IEnumerator

        IEnumerator<SessionID> IEnumerable<SessionID>.GetEnumerator() => sessions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => sessions.GetEnumerator();

        #endregion IEnumerator
    }

    public class Session : IEnumerable<SessionElementID>
    {
        private static int nextSessionID = 1;
        public readonly int id;

        public List<SessionElementID> sessionElements;
        public JsonObject envVals;

        //Explicitly created sessions are added to the Session dictionary
        public Session()
        {
            id = nextSessionID++;
            sessionElements = new List<SessionElementID>();
            envVals = new JsonObject();

            ProtocolManager.sessionDictionary.Add(id, this);
        }

        //Deserialized Sessions are not added to the Session dictionary
        public Session(int id)
        {
            this.id = id;
            if (nextSessionID <= id)
            {
                nextSessionID = id + 1;
            }

            //Should be assigned by constructing caller
            envVals = null;
        }

        public int Count => sessionElements.Count;
        public SessionElement this[int i] => sessionElements[i].Element;

        public void Add(SessionElement element)
        {
            //It was supremely convenient for constructing tasks to allow for (and block)
            //  null session elements
            if (element != null)
            {
                sessionElements.Add(element);
            }
        }

        public void AddRange(IEnumerable<SessionElement> elements)
        {
            foreach (SessionElement element in elements)
            {
                //It was supremely convenient for constructing tasks to allow for (and block)
                //  null session elements
                if (element != null)
                {
                    sessionElements.Add(element);
                }
            }
        }

        public static void HardClear()
        {
            nextSessionID = 1;
        }

        #region IEnumerator

        IEnumerator<SessionElementID> IEnumerable<SessionElementID>.GetEnumerator() => sessionElements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => sessionElements.GetEnumerator();

        #endregion IEnumerator
    }

    public readonly struct ProtocolID
    {
        public readonly int id;
        public Protocol Protocol => ProtocolManager.protocolDictionary.ContainsKey(id) ?
            ProtocolManager.protocolDictionary[id] : null;

        public ProtocolID(int id)
        {
            this.id = id;
        }

        public static implicit operator ProtocolID(Protocol protocol) => new ProtocolID(protocol.id);
        public static implicit operator ProtocolID(int id) => new ProtocolID(id);
    }

    public readonly struct SessionID
    {
        public readonly int id;
        public Session Session => ProtocolManager.sessionDictionary.ContainsKey(id) ?
            ProtocolManager.sessionDictionary[id] : null;

        public SessionID(int id)
        {
            this.id = id;
        }

        public static implicit operator SessionID(Session session) => new SessionID(session.id);
        public static implicit operator SessionID(int id) => new SessionID(id);
    }

    public readonly struct SessionElementID
    {
        public readonly int id;
        public SessionElement Element => ProtocolManager.sessionElementDictionary.ContainsKey(id) ?
            ProtocolManager.sessionElementDictionary[id] : null;

        public SessionElementID(int id)
        {
            this.id = id;
        }

        public static implicit operator SessionElementID(SessionElement element) => new SessionElementID(element.id);
        public static implicit operator SessionElementID(int id) => new SessionElementID(id);
    }
}