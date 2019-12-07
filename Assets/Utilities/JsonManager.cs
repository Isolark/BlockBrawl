using UnityEngine;

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLManager : MonoBehaviour {
    public static XMLManager ins;

    void Awake() {
        ins = this;
    }
}

public class JsonManager {
    void Awake() {

    }
}

