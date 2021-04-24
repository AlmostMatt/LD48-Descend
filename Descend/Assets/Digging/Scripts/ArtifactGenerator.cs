using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactGenerator : MonoBehaviour
{
    public GameObject artifactObject;

    public int minX;
    public int maxX;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 3; ++i)
        {
            PlaceArtifact();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceArtifact()
    {
        int minDepth = 0;
        int maxDepth = 10;
        int x = Random.Range(minX, maxX);
        int depth = Random.Range(minDepth, maxDepth);

        Instantiate(artifactObject, new Vector3(x, -depth, -1), Quaternion.identity);
    }
}
