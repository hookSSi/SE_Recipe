<?php
    require 'DBManager.php';

    /// todo
    /// 레시피 열람기록 등록 함수
    /// 레시피 열람기록 삭제 함수
    /// 코드 정리
    /// 테스트 케이스 정리
    /// 문서 구조 변경        

    /// 레시피를 관리하는 클래스
    class RecipeManager
    {
        private $db;
        private $recipeInfoTableName;
        private $recipeIngredientTableName;
        private $recipeProcessTableName;

        function __construct()
        {
            $this->db = new DBManager();
            $this->recipeTableName = "RECIPE_INFO";
            $this->recipeIngredientTableName = "RECIPE_INGREDIENT_INFO";
            $this->recipeProcessTableName = "RECIPE_PROCESS_INFO";
            $this->userHistoryTableName = "USER_HISTORY";
        }

        /// 레시피 검색
        function SearchRecipe($search_keyword, $option)
        {
            $sql = "";

            switch($option)
            {
                case "이름": // 레시피 이름 검색
                    $sql = "SELECT * FROM {$this->recipeTableName} WHERE RECIPE_NM_KO LIKE '{$search_keyword}%'";
                break;
                case "난이도": // 레시피 난이도 검색
                    $sql = "SELECT * FROM {$this->recipeTableName} WHERE LEVEL_NM LIKE '{$search_keyword}%'";
                break;
                case "재료": // 레시피 재료 검색
                    $sql = "SELECT DISTINCT recipe.*
                            FROM {$this->recipeTableName} AS recipe 
                            JOIN {$this->recipeIngredientTableName} AS ingredient 
                            WHERE ingredient.IRDNT_NM LIKE '{$search_keyword}%' 
                            AND recipe.RECIPE_ID = ingredient.RECIPE_ID";
                break;
            }

            $result = ($this->db)->QueryDB($sql);

            if(mysqli_num_rows($result) == 0)
            {
                echo "결과가 없습니다.";
            }
            else
            {
                while($row = mysqli_fetch_assoc($result)) 
                {
                    echo json_encode($row, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT)."\n";
                }
            }
        }

        /// 레시피 상세 정보 불러오기
        function LoadRecipeIngredientInfo($recipeID)
        {
            $sql = "SELECT * FROM {$this->recipeIngredientTableName} WHERE RECIPE_ID='{$recipeID}'";

            $result = ($this->db)->QueryDB($sql);
            $row = mysqli_fetch_row($result);
            
            if(mysqli_num_rows($result) == 0)
            {
                echo "결과가 없습니다.";
            }
            else
            {
                echo json_encode($row, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT)."\n";
            }
        }

        /// 레시피 요리 과정 불러오기
        function LoadRecipeProcess($recipeID)
        {
            $sql = "SELECT * FROM {$this->recipeProcessTableName} WHERE RECIPE_ID='{$recipeID}' ORDER BY COOKING_NO";

            $result = ($this->db)->QueryDB($sql);
            $row = mysqli_fetch_row($result);

            $result = ($this->db)->QueryDB($sql);

            if(mysqli_num_rows($result) == 0)
            {
                echo "결과가 없습니다.";
            }
            else
            {
                while($row = mysqli_fetch_array($result)) 
                {
                    echo json_encode($row, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT)."\n";
                }
            }
        }

        /// 레시피 열람 기록 저장
        function SaveRecipe($userID, $recipeID)
        {
            $sql = "INSERT INTO {$this->userHistoryTableName} VALUES ('{$userID}', '{$recipeID}')";
        }

        /// 레시피 열람 기록 삭제
        function RemoveRecipe($userID, $recipeID)
        {
            $sql = "DELETE FROM {$this->userHistoryTableName} WHERE USER_ID='{$userID}' AND RECIPE_ID='{$recipeID}'";
        }

        /// 레시피 열람 기록 불러오기
        function LoadRecipeSaved($userID)
        {
            
        }
    }
?>