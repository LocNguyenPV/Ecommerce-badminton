pipeline {
    agent any
    environment {

        // Lấy Build Number làm Tag
        BE_IMAGE_NAME = "ecommerce-be"
        FE_IMAGE_NAME = "ecommerce-fe"
        IMAGE_TAG = "${env.BUILD_NUMBER}"

        // Nếu namespace của Harbor là 'devops-tools', service thường là:
        HARBOR_HOST = 'registry.codebyluke.io.vn'
        HARBOR_PROJECT = 'ecommerce-badminton'
        GITLAB_REPO_URL = 'https://gitlab.codebyluke.io.vn/root/ecommerce-badminton-hub.git'
        
        HARBOR_CREDS_ID = 'harbor-creds'
        GIT_CREDS_ID = 'gitlab-creds'


         // Thông tin GCP
        PROJECT_ID = "devops-476202"
        LOCATION   = "asia-southeast1"
        REPO_NAME  = "ecommerce-repo"
       

          // Đường dẫn đầy đủ của Registry
        REGISTRY_URL = "${LOCATION}-docker.pkg.dev/${PROJECT_ID}/${REPO_NAME}"
        // ID của Secret File chứa JSON Key trong Jenkins
        GCP_CREDS_ID = "gcp-service-account-key"
    }
    
   stages {
        stage('Checkout & Build') {
            steps {
                git branch: 'main', credentialsId: "${GIT_CREDS_ID}", url: "${GITLAB_REPO_URL}"
                script {
                    dir('ECommerce.ProductManagement') { sh "docker build -t ${BE_IMAGE_NAME}:${IMAGE_TAG} ." }
                    dir('ecommerce-badminton-fe') { sh "docker build -t ${FE_IMAGE_NAME}:${IMAGE_TAG} ." }
                }
            }
        }
        
        stage('Push to Harbor & Update Manifest') {
            steps {
                script {
                    // 1. Push lên Harbor (Lab environment)
                    withCredentials([usernamePassword(credentialsId: "${HARBOR_CREDS_ID}", usernameVariable: 'USER', passwordVariable: 'PASS')]) {
                        sh "docker login ${HARBOR_HOST} -u $USER -p $PASS"
                        sh "docker tag ${BE_IMAGE_NAME}:${IMAGE_TAG} ${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                        sh "docker tag ${FE_IMAGE_NAME}:${IMAGE_TAG} ${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                        sh "docker push ${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                        sh "docker push ${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                    }
                    
                    // 2. Update Manifest để ArgoCD deploy lên cụm Lab On-premise
                    sh "sed -i 's|${BE_IMAGE_NAME}:.*|${BE_IMAGE_NAME}:${IMAGE_TAG}|g' k8s/03-backend.yaml"
                    sh "sed -i 's|${FE_IMAGE_NAME}:.*|${FE_IMAGE_NAME}:${IMAGE_TAG}|g' k8s/04-frontend.yaml"
                    
                    withCredentials([usernamePassword(credentialsId: GIT_CREDS_ID, usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        def repoClean = env.GITLAB_REPO_URL.replace("https://", "")
                        sh """
                            git config user.email "jenkins@bot.com"
                            git config user.name "Jenkins Bot"
                            git add k8s/
                            git commit -m 'Deploy to Lab - Build ${IMAGE_TAG}' || echo "No changes"
                            git push https://${GIT_USER}:${GIT_PASS}@${repoClean} HEAD:main
                        """
                    }
                }
            }
        }

    // stage('QA Confirmation') {
    //     steps {
    //         script {
    //             def qaResult = input(
    //                 message: "Xác nhận kết quả kiểm thử",
    //                 parameters: [
    //                     string(name: 'QA_NAME', defaultValue: '', description: 'Tên người kiểm thử'),
    //                     choice(name: 'TEST_STATUS', choices: ['PASSED', 'FAILED'], description: 'Kết quả test')
    //                 ],
    //                 submitter: "luke,qa-lead"
    //             )
                
    //             // Nếu chọn FAILED, ta chủ động hủy pipeline luôn
    //             if (qaResult['TEST_STATUS'] == 'FAILED') {
    //                 error "Pipeline bị dừng do QA báo kết quả Test thất bại!"
    //             }
                
    //             echo "QA ${qaResult['QA_NAME']} đã phê duyệt bản build này."
    //         }
    //     }
    // }

        stage('Push to GCP Artifact Registry') {
            steps {
                script {
                    echo '--- PROMOTING IMAGES TO PRODUCTION (GCP) ---'
                    withCredentials([file(credentialsId: "${GCP_CREDS_ID}", variable: 'GCP_KEY')]) {
                        sh "gcloud auth activate-service-account --key-file=${GCP_KEY}"
                        sh "gcloud auth configure-docker ${LOCATION}-docker.pkg.dev --quiet"
                        
                        def beGCP = "${REGISTRY_URL}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                        def feGCP = "${REGISTRY_URL}/${FE_IMAGE_NAME}:${IMAGE_TAG}"

                        sh "docker tag ${BE_IMAGE_NAME}:${IMAGE_TAG} ${beGCP}"
                        sh "docker tag ${FE_IMAGE_NAME}:${IMAGE_TAG} ${feGCP}"

                        sh "docker push ${beGCP}"
                        sh "docker push ${feGCP}"
                    }
                }
            }
        }
    }
}