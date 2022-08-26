using UnityEngine; 
using System.Collections;


public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;       //Time it will take object to move, in seconds.
    public LayerMask blockingLayer;     //Layer on which collision will be checked.


    private BoxCollider2D boxCollider;  //The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;           //The Rigidbody2D component attached to this object.
    private float inverseMoveTime;      //Used to make movement more efficient.
    private bool isMoving;


    //Protected, virtual functions can be overridden by inheriting classes.
    protected virtual void Start ()
    {
        boxCollider = GetComponent <BoxCollider2D> ();
        rb2D = GetComponent <Rigidbody2D> ();
        inverseMoveTime = 1f / moveTime;
    }


    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2 (xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast (start, end, blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null && !isMoving)
        {
            StartCoroutine (SmoothMovement (end));
            return true;
        }

        return false;
    }


    protected IEnumerator SmoothMovement (Vector3 end)
    {
		isMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            // MovePosition lags in the editor
            // Need to use transform.position
            // rb2D.MovePosition(newPostion);
            rb2D.transform.position = newPostion;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        rb2D.MovePosition (end);
        isMoving = false;
    }


    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move (xDir, yDir, out hit);

        if(hit.transform == null)
            //If nothing was hit, return and don't execute further code.
            return;

        T hitComponent = hit.transform.GetComponent <T> ();

        if(!canMove && hitComponent != null)
            OnCantMove (hitComponent);
    }


    //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
    //OnCantMove will be overriden by functions in the inheriting classes.
    protected abstract void OnCantMove <T> (T component)
        where T : Component;
}
