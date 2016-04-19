using UnityEngine;
using System.Collections;
using Vuforia;

public class AttachContentToTrackables : MonoBehaviour
{
    /*
    // Get the number of Trackables defined in the data set.
    DataSet dataSet = GameObject.Find("ARCamera").GetComponent("D");


    int numTrackables = dataSet.GetNumTrackables();

    // Loop over all Trackables.

    for (int i = 0; i < numTrackables; ++i)
    {
        DataSetTrackableBehaviour dstb = dataSet.GetTrackable(i);
        GameObject go = dstb.gameObject;

        // Add a Trackable event handler to the Trackable.
        // This Behaviour handles Trackable lost/found callbacks.

        go.AddComponent<DefaultTrackableEventHandler>();

        // Create a cube object.

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Attach the cube to the Trackable and make sure it has a proper size.

        cube.transform.parent = dstb.transform;
        cube.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        cube.transform.localPosition = new Vector3(0.0f, 0.35f, 0.0f);
        cube.transform.localRotation = Quaternion.identity;
        cube.active = true;
        dstb.gameObject.active = true;
    }*/
}