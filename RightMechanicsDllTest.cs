using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading;

using UnityEngine;
using UnityEngine.Events;


namespace RightMechanicsDLL
{
    public class Data3D
    {
        public float x;
        public float y;
        public float z;

        public Data3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class RightMechanicsDllTest
    {
        public RightMechanicsDllTest() { }

        public string SayHello()
        {
            return "Hello there!!!";
        }

        public void InvokeDoSomething(int times)
        {
            for (int i = 0; i < times; i++)
                somethingInvoker.Invoke();
        }

        public void InvoicePositionData(int times)
        {
            Random random = new Random();
            
            for (int i=0; i<times; i++)
            {
                Data3D data = new Data3D((float)(random.NextDouble() * (10 - 0) + 0), (float)(random.NextDouble() * (10 - 0) + 0), (float)(random.NextDouble() * (10 - 0) + 0));
                positionDataInvoker.Invoke(data);
            }
        }

        public delegate void SomethingHandler();
        private SomethingHandler somethingInvoker = () => { };

        public event SomethingHandler OnSomething
        {
            add { somethingInvoker += value; }
            remove { somethingInvoker -= value; }
        }

        public delegate void PositionDataHandler(Data3D data);
        private PositionDataHandler positionDataInvoker = (Data3D data) => { };

        public delegate void RotationDataHandler(Data3D data);
        private RotationDataHandler rotationDataInvoker = (Data3D data) => { };

        public event PositionDataHandler OnPositionData
        {
            add { positionDataInvoker += value; }
            remove { positionDataInvoker -= value; }
        }

        public event RotationDataHandler OnRotationData
        {
            add { rotationDataInvoker += value; }
            remove { rotationDataInvoker -= value; }
        }
    }


}
