using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingManager : MonoBehaviour {
	//캐릭터 이동 관련 스크립트


	//변수 선언
	//컴포넌트 관련 변수
	private BoxCollider2D boxCollider;
    private LayerMask layerMask;
    private Animator animator;
	//이동관련 변수 
	public float speed;
	public int walkCount;
    private int currentWalkCount;

	//캐릭터 좌표 변수
	private Vector3 vector;


	//대쉬관련 변수
	public float Dash;
	private float applyDash;

	//이동 불/가능 변수
    private bool applyRunFlag = false;
    private bool canMove = true;

	//캐릭터 게임 오브젝트를 저장하기 위한 정적변수
	//반복적으로 사용되는 게임 오브젝트를 생성, 관리하기 쉽다
	public static MovingManager instance;

	//초기화시 사용되는 함수, 단 한번만 호출
	private void Awake()
    {
		//함수 실행시 instance 변수가 값을 가지고 있지 않다면 현재 오브젝트를 파괴하지 않음
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
			//이후 정적변수 instance에 스크립트가 적용된 오브젝트를 저장
            instance = this;
        }
        else
        {
			//만약 instance에 값이 존재한다면 해당 오브젝트를 제거하여 중복으로 존재하는 것을 방지
            Destroy(this.gameObject);
        }
    }

	//이동 코루틴
	IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
			//좌측 쉬프트키를 동시에 눌렀을 경우 이동속도가 증가
            if (Input.GetKey(KeyCode.LeftShift))
            {
                applyDash = Dash;
                applyRunFlag = true;
            }
            else
            {
                applyDash = 0;
                applyRunFlag = false;
            }

			//현재의 x,y,z의 좌표값을 vector 변수에 저장
            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

			//상하, 좌우 중복 입력 방지 조건문,
            if (vector.x != 0)
                vector.y = 0;

			//애니메이터 실행
			//좌우 이동 애니메이션 실행
            animator.SetFloat("DirX", vector.x);
			//상하 이동 애니메이션 실행
            animator.SetFloat("DirY", vector.y);
			//대기시 에니메이션 실행
            animator.SetBool("Walking", true);

			//currentWalkCount가 walkCount 변수와 같은 값이 되었을때 반복문을 종료
            while (currentWalkCount < walkCount)
            {
				//
                transform.Translate(vector.x * (speed + applyDash), vector.y * (speed + applyDash), 0);
                if (applyRunFlag)
                    currentWalkCount++;
                currentWalkCount++;

				//주어진 값만큼 코루틴의 수행을 중단
                yield return new WaitForSeconds(0.01f);
            }
			//반복문 종료시 currentWalkCount 변수 초기화
            currentWalkCount = 0;

        }
		//반복문 종료시 변수 값 초기화
        animator.SetBool("Walking", false);
        canMove = true;
        
    }

    //매 프레임마다 호출되는 함수
    void Update()
    {
		//이동 가능시 
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());//코루틴 실행
            }
        }

    }
}