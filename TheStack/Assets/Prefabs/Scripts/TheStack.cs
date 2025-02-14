using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    private const float BoundSize = 3.5f;
    private const float MovingBoundSize = 3f;
    private const float StackMovingSpeed = 5.0f;
    private const float BloxkMaovingSpeed = 3.5f;
    private const float ErrorMargin = 0.1f;

    public GameObject orignBlock = null;

    private Vector3 prevBlockPosition;
    private Vector3 desiredPosition;
    private Vector3 stackBounds = new Vector3(BoundSize, BoundSize);

    Transform lastBlock = null;
    float blockTransition = 0f;
    float secondaryPosition = 0f;

    int stackcount = -1;
    int comboCount = 0;

    public Color prevColor;
    public Color nextColor;
   

    // Start is called before the first frame update
    void Start()
    {
        if(orignBlock == null)
        {
            Debug.Log("OriginBlock is Null");
            return;
        }

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down;
        Spawn_Block();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Spawn_Block();
        }
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed*Time.deltaTime);
    }

    bool Spawn_Block()
    {
        if(lastBlock != null)
        {
            prevBlockPosition = lastBlock.localPosition;
        }
        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(orignBlock);

        if(newBlock == null)
        {
            Debug.Log("NewBlock Instantiate Failed");
            return false;
        }

        ColorChange(newBlock);

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition+Vector3.up;
        newTrans.localRotation = Quaternion.identity;
        newTrans.localScale = new Vector3(stackBounds.x,1, stackBounds.y);

        stackcount++;

        desiredPosition = Vector3.down*stackcount;
        blockTransition = 0f;

        lastBlock = newTrans;

        return true;

    }
    Color GetRandomColor()
    {
        float r = Random.Range(100f, 250f)/255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        Color appltColor = Color.Lerp(prevColor, nextColor, (stackcount % 11) / 10f);
        Renderer rn = go.GetComponent<Renderer>();

        if(rn == null)
        {
            Debug.Log("Lenderer is Null");
            return;
        }
        rn.material.color = appltColor;
        Camera.main.backgroundColor = appltColor-new Color(0.1f,0.1f,0.1f);

        if(appltColor.Equals(nextColor)==true)
        {
            prevColor = nextColor;
            nextColor = GetRandomColor();
        }
    }
}
