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

    bool isMovingX = true; //이동 방향에 따라서 X축과 Z축으로 이동하기 위해

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
        Spawn_Block();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                Debug.Log("GameOver");
            }
            
        }
        MoveBlock();
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

        isMovingX = !isMovingX;
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

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BloxkMaovingSpeed;
        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 3; // 0부터 우리가 지정해준 사이즈까지를 순환하는 값
        
        if(isMovingX)
        {
            lastBlock.localPosition = new Vector3(movePosition*MovingBoundSize,stackcount,secondaryPosition);   

        }
        else
        {
            lastBlock.localPosition = new Vector3(secondaryPosition, stackcount, -movePosition * MovingBoundSize);

        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPositon = lastBlock.localPosition;
        if(isMovingX)
        {
            float deltax = prevBlockPosition.x - lastPositon.x; // 잘려나가는 크기
            bool isNegativeNum = (deltax<0)? true : false; 


            deltax = Mathf.Abs(deltax);
            if (deltax > ErrorMargin)
            {
                stackBounds.x -= deltax;
                if(stackBounds.x <=0)
                {
                    return false;
                }
                float middle = (prevBlockPosition.x + lastPositon.x) / 2f;
                lastBlock.localScale = new Vector3(stackBounds.x,1,stackBounds.y);

                Vector3 temPosition = lastBlock.localPosition;
                temPosition.x = middle;
                lastBlock.localPosition = lastPositon = temPosition;

                float rubbleHalfScale = deltax / 2f;
                CreateRubble(
                    new Vector3(isNegativeNum? lastPositon.x+ stackBounds.x / 2 + rubbleHalfScale:
                    lastPositon.x -stackBounds.x / 2 - rubbleHalfScale, lastPositon.y, lastPositon.z),new Vector3(deltax,1,stackBounds.y));
            }
            else
            {
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }

        }
        else
        {
            float deltaZ = prevBlockPosition.z - lastPositon.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false;

            deltaZ = Mathf.Abs(deltaZ);
            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if(stackBounds.y <=0)

                {
                    return false;
                }
                float middle = (prevBlockPosition.z + lastPositon.z) / 2f;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition =lastPositon =tempPosition;

                float rubbleHalfScale = deltaZ / 2f;
                CreateRubble(
                    new Vector3(lastPositon.x, lastPositon.y,isNegativeNum ? lastPositon.z + stackBounds.y / 2 + rubbleHalfScale :
                    lastPositon.z - stackBounds.y / 2 - rubbleHalfScale ), new Vector3(stackBounds.x,1,deltaZ));

            }
            else
            {
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;
        return true;
    }

    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject);
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localScale = pos;
        go.transform.localRotation = Quaternion.identity;

        go.AddComponent<Rigidbody>();
        go.name = "Rubble";

    }
}
