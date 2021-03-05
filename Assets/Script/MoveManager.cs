using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{

    //변수선언
	//플레이어 이동속도
    public float speed; 
    public int walkCount;
    private int currentWalkCount;

	//캐릭터 좌표(x,y,z) 변수
    private Vector3 vector;

	//가속도 변수
    public float dash;
	//이동속도 연산에 사용 되는 변수
    private float dashSpeed;
	//가속도 체크
    private bool dashFlag = false;
	//이동 
    private bool moveOn = true;


    //컴포넌트 호출
    private BoxCollider2D boxCollider;
    private LayerMask layerMask;
    private Animator animator;


    // Use this for initialization
    void Start()
    {

    }

	//이동 코루틴
	IEnumerator MoveCoroutine()
    {
		//반복문이 돌며 방향키를 입력받음
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
			//왼쪽 쉬프트 키 동시 입력시 가속
            if (Input.GetKey(KeyCode.LeftShift))
            {
                dashSpeed = dash;
                dashFlag = true;
            }
			//왼쪽 쉬프트 키 미입력
            else
            {
				//변수 초기화ㄴ
                dashSpeed = 0;
                dashFlag = false;
            }

			//현재 캐릭터의 좌표를 변수에 저장
            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);

            if (vector.x != 0)
                vector.y = 0;

			//참조된 애니메이터 파라미터에 값 저장
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);
			//애니메이터 상태 변환
            animator.SetBool("Walking", true);

            while (currentWalkCount < walkCount)
            {

                transform.Translate(vector.x * (speed + dashSpeed), vector.y * (speed + dashSpeed), 0);
                if (dashFlag)
                    currentWalkCount++;
                currentWalkCount++;
				//코루틴 지연시간 설정, 애니메이션 프레임 오류 방지
                yield return new WaitForSeconds(0.01f);
            }
			//변수 초기화
            currentWalkCount = 0;

        }
		//대기 에니메이션(Standing) 설정 
        animator.SetBool("Walking", false);
        moveOn = true;
    }

    // Update is called once per frame
    void Update()
    {
		//moveOn 변수의 값이 참일 때만 실행
        if (moveOn)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
				//코루틴이 실행되는 동안 중복 입력 방지
                moveOn = false;
				//코루틴 실행
                StartCoroutine(MoveCoroutine());
            }
        }

    }
}

