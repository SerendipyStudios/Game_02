using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkedObject : PhotonView, IPunObservable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Make a gameObject networked
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    //    if (stream.IsWriting)
    //    {
    //        Vector3 pos = transform.localPosition;
    //        stream.Serialize(ref pos);
    //    }
    //    else
    //    {
    //        Vector3 pos = Vector3.zero;
    //        stream.Serialize(ref pos);  // pos gets filled-in. must be used somewhere
    //    }
    }

    //Rpc method
    //[PunRPC]
    //public void OnAwakeRPC(byte myParameter)
    //{
    //    Debug.Log(string.Format("RPC: 'OnAwakeRPC' Parameter: {0} PhotonView: {1}", myParameter, this));
    //}

    //Rpc call
    //public void CallOtherRPC(PhotonView photonView)
    //{
    //    photonView.RPC("OnAwakeRPC", RpcTarget.All, (byte)1);
    //}
    
    //Event call
    //PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
}
