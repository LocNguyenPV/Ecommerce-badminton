pipeline {
    agent any
    environment {
        BE_IMAGE_NAME = "ecommerce-be"
        FE_IMAGE_NAME = "ecommerce-fe"
        IMAGE_TAG = "${env.BUILD_NUMBER}"

        HARBOR_HOST = 'registry.codebyluke.io.vn'
        HARBOR_PROJECT = 'ecommerce-badminton'
        GITLAB_REPO_CODE_URL = 'https://gitlab.codebyluke.io.vn/hybrid-cloud/ecommerce-badminton-hub.git'
        GITLAB_REPO_MANIFEST_URL = 'https://gitlab.codebyluke.io.vn/hybrid-cloud/manifest.git'
        
        HARBOR_CREDS_ID = 'harbor-creds'
        GIT_CREDS_ID = 'gitlab-creds'

        PROJECT_ID = "devops-476202"
        LOCATION   = "asia-southeast1"
        REPO_NAME  = "ecommerce-repo"
        REGISTRY_URL = "${LOCATION}-docker.pkg.dev/${PROJECT_ID}/${REPO_NAME}"
        GCP_CREDS_ID = "gcp-service-account-key"
    }
    
    stages {
        stage('Checkout & Build') {
            steps {
                git branch: 'main', credentialsId: "${GIT_CREDS_ID}", url: "${GITLAB_REPO_CODE_URL}"
                script {
                    // Build một lần duy nhất, dùng cho cả 2 môi trường
                    dir('ECommerce.ProductManagement') { sh "docker build -t ${BE_IMAGE_NAME}:${IMAGE_TAG} ." }
                    dir('ecommerce-badminton-fe') { sh "docker build -t ${FE_IMAGE_NAME}:${IMAGE_TAG} ." }
                }
            }
        }
        
        stage('Push to Harbor & Lab Deploy') {
            steps {
                // Checkout repo Manifest vào thư mục riêng để không đè lên repo Code
                dir('manifest-repo') {
                    git branch: 'main', credentialsId: "${GIT_CREDS_ID}", url: "${GITLAB_REPO_MANIFEST_URL}"
                }
                
                script {
                    // 1. Push lên Harbor
                    withCredentials([usernamePassword(credentialsId: "${HARBOR_CREDS_ID}", usernameVariable: 'USER', passwordVariable: 'PASS')]) {
                        sh "docker login ${HARBOR_HOST} -u $USER -p $PASS"
                        sh "docker tag ${BE_IMAGE_NAME}:${IMAGE_TAG} ${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                        sh "docker tag ${FE_IMAGE_NAME}:${IMAGE_TAG} ${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                        sh "docker push ${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                        sh "docker push ${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                    }
                    
                    // 2. Kustomize cho Lab
                    docker.image('line/kubectl-kustomize').inside {
                        dir('manifest-repo/k8s/overlays/lab') {
                            sh "kustomize edit set image ecommerce-be=${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                            sh "kustomize edit set image ecommerce-fe=${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                        }
                    }
                    
                    // 3. Push Git Manifest Lab
                    dir('manifest-repo') {
                        withCredentials([usernamePassword(credentialsId: GIT_CREDS_ID, usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                            def repoClean = env.GITLAB_REPO_MANIFEST_URL.replace("https://", "")
                            sh """
                                git config user.email "jenkins@bot.com"
                                git config user.name "Jenkins Bot"
                                git add k8s/overlays/lab/
                                git commit -m 'GitOps: Deploy to Lab - Build ${IMAGE_TAG}' || echo "No changes"
                                git push https://${GIT_USER}:${GIT_PASS}@${repoClean} HEAD:main
                            """
                        }
                    }
                }
            }
        }

        // Nếu bạn mở khóa stage QA, nó sẽ nằm ở đây
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

        stage('Push to GCP & GKE Deploy') {
            steps {
                script {
                    // 1. Push lên GCP
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

                    // 2. Kustomize cho GKE
                    docker.image('line/kubectl-kustomize').inside {
                        dir('manifest-repo/k8s/overlays/gke') {
                            sh "kustomize edit set image ecommerce-be=${REGISTRY_URL}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                            sh "kustomize edit set image ecommerce-fe=${REGISTRY_URL}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                        }
                    }

                    // 3. Push Git Manifest GKE
                    dir('manifest-repo') {
                        withCredentials([usernamePassword(credentialsId: GIT_CREDS_ID, usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                            def repoClean = env.GITLAB_REPO_MANIFEST_URL.replace("https://", "")
                            sh """
                                git add k8s/overlays/gke/
                                git commit -m 'GitOps: Deploy to GKE - Build ${IMAGE_TAG}' || echo "No changes"
                                git push https://${GIT_USER}:${GIT_PASS}@${repoClean} HEAD:main
                            """
                        }
                    }
                }
            }
        }
    }
}