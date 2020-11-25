<?php
    require 'DBManager.php';

    /// 유저의 로그인, 회원관리를 관리할 클래스
    class UserManager
    {
        private $db;
        private $userTableName;
        public $response;

        function __construct()
        {
            $this->db = new DBManager();
            $this->userTableName = "USER_INFO";

            $this->response = array();
            $this->response['userID'] = "";
            $this->response['userPW'] = "";
            $this->response['userEmail'] = "";
            $this->response['state'] = false;
            $this->response['desc'] = "";
        }
        
        /// 회원가입 함수
        public function Register($registerID, $registerPW, $registerEmail)
        {
            $this->response['userID'] = $registerID;
            $this->response['userPW'] = $registerPW;
            $this->response['userEmail'] = $registerEmail;

            if($this->IsValidRegisterID($registerID))
            {
                $sql = "INSERT INTO {$this->userTableName} 
                        (USER_ID, USER_PASSWORD, USER_EMAIL) 
                        VALUES ('{$registerID}', '{$registerPW}', '{$registerEmail}')";

                $result = ($this->db)->QueryDB($sql);

                if($result === false)
                {
                    $this->response['state'] = false;
                    $this->response['desc'] = "회원가입실패";
                }
                else
                {
                    $this->response['state'] = true;
                    $this->response['desc'] = "회원가입성공";
                }
            }
            else
            {
                $this->response['state'] = false;
                $this->response['desc'] = "중복ID오류";
            }

            return $this->response;
        }

        /// 회원가입시 ID 중복 여부 검사
        public function IsValidRegisterID($registerID)
        {
            $sql = "SELECT * FROM {$this->userTableName} WHERE USER_ID = '{$registerID}'";
            $result = ($this->db)->QueryDB($sql);
            
            // 같은 ID가 없을 경우 true 반환
            if(mysqli_num_rows($result) == 0)
            {
                return true;
            }
            else
                return false;
        }

        /// 로그인 하는 함수
        public function Login($loginID, $loginPW)
        {
            $this->response['userID'] = $loginID;
            $this->response['userPW'] = $loginPW;

            $sql = "SELECT * FROM {$this->userTableName} WHERE USER_ID = '{$loginID}'";

            $result = ($this->db)->QueryDB($sql);
            $row = mysqli_fetch_row($result);

            if($row)
            {
                $userID = $row[0];
                $userPW = $row[1];
 
                if(password_verify($loginPW, $userPW))
                {
                    // 로그인 성공
                    $this->response['state'] = true;
                    $this->response['userEmail'] = $row[2];
                    $this->response['desc'] = "로그인성공";
                }
                else
                {
                    // 비밀번호 오류
                    $this->response['state'] = false;
                    $this->response['desc'] = "비밀번호오류";
                }
            }
            else
            {
                // ID가 존재하지 않음
                $this->response['state'] = false;
                $this->response['desc'] = "ID오류";
            }

            return $this->response;
        }
    }
?>