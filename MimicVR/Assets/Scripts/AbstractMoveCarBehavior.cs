using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AbstractMoveCarBehavior : MonoBehaviour
{
    [SerializeField]
    protected SocketMoveCmd socket;

    [SerializeField]
    protected LocalMoveCmd local;

    protected RobotCommandInput moveCmd;

    void Start()
    {
        
        if (socket != null)
        {
            moveCmd = socket;
        }
        else
        {
            moveCmd = local;
        }

        // write coroutine script here.
        cStart();
    }

    public abstract void cStart();

}

