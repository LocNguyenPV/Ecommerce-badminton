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
        GIT_CREDS_ID = 'gitlab-pat-creds'

        PROJECT_ID = "devops-485312"
        LOCATION   = "asia-southeast1"
        REPO_NAME  = "ecommerce-repo"
        REGISTRY_URL = "${LOCATION}-docker.pkg.dev/${PROJECT_ID}/${REPO_NAME}"
        GCP_CREDS_ID = "gcp-service-account-key"
    }
    
    stages {
        stage('Checkout & Build') {
            steps {
                git branch: 'main', credentialsId: "${GIT_CREDS_ID}", url: "${GITLAB_REPO_CODE_URL}"
                script { // Bọc parallel vào đây
                    parallel(
                        "Build Backend": {
                            dir('ECommerce.ProductManagement') { sh "docker build -t ${BE_IMAGE_NAME}:${IMAGE_TAG} ." }
                        },
                        "Build Frontend": {
                            dir('ecommerce-badminton-fe') { sh "docker build -t ${FE_IMAGE_NAME}:${IMAGE_TAG} ." }
                        }
                    )
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
                    
                    // 2. Kustomize cho on-premise
                    docker.image('line/kubectl-kustomize').inside {
                        dir('manifest-repo/ecommerce/overlays/on-premise') {
                            sh "kustomize edit set image ecommerce-be=${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                            sh "kustomize edit set image ecommerce-fe=${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                        }
                    }

                    // 3. Push Git Manifest (SỬ DỤNG LẠI PAT ĐỂ PUSH)
                    withCredentials([usernamePassword(credentialsId: "${GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                        dir('manifest-repo') {
                            sh """
                                git config user.email "jenkins@bot.com"
                                git config user.name "Jenkins Bot"
                                
                                
                                git add ecommerce/overlays/on-premise/
                                git commit -m 'GitOps: Deploy to On-premise - Build ${IMAGE_TAG}' || echo "No changes to commit"
                                
                                # QUAN TRỌNG: Gán Token vào URL để Push qua HTTPS
                                # URL mẫu: https://user:token@git.codebyluke.io.vn/hybrid-cloud/manifest.git
                                git push https://${GIT_USER}:${GIT_TOKEN}@${GITLAB_REPO_MANIFEST_URL.replace('http://', '').replace('https://', '')} HEAD:main
                            """
                        }
                    }
                }
            }
        }

        stage('Generate Release Notes') {
            steps {
                script {
                    // Lấy danh sách các commit từ lần build trước đến hiện tại
                    // Định dạng: - Nội dung commit (Tên tác giả)
                    def changeLogSets = currentBuild.changeSets
                    def notes = ""
                    
                    if (changeLogSets.isEmpty()) {
                        notes = "- Không có thay đổi mã nguồn (có thể chỉ build lại hoặc update manifest)."
                    } else {
                        for (int i = 0; i < changeLogSets.size(); i++) {
                            def entries = changeLogSets[i].items
                            for (int j = 0; j < entries.length; j++) {
                                def entry = entries[j]
                                notes += "- ${entry.msg} _(by ${entry.author.fullName})_\n"
                            }
                        }
                    }
                    // Lưu vào biến môi trường để dùng cho stage sau
                    env.RELEASE_NOTES = notes
                }
            }
        }

        stage('Notify QA') {
        steps {
            withCredentials([
                string(credentialsId: 'TELEGRAM_BOT_TOKEN', variable: 'TOKEN'),
                string(credentialsId: 'TELEGRAM_CHAT_ID', variable: 'CHAT')
            ]) {
                script {
                    def message = """
                    🔔 *YÊU CẦU PHÊ DUYỆT PIPELINE*
                    
                    *Dự án:* ${env.JOB_NAME}
                    *Build số:* #${env.BUILD_NUMBER}
                    *Môi trường:* Hybrid Cloud (On-premise)
                    
                    📝 *Cập nhật mới (Release Notes):*
                    ${env.RELEASE_NOTES}
                    
                    *Trạng thái:* Đang chờ QA xác nhận kết quả.
                    👉 [Nhấn vào đây để Approve](${env.BUILD_URL}input)
                    """.stripIndent()

                    sh "curl -s -X POST https://api.telegram.org/bot${TOKEN}/sendMessage -d chat_id=${CHAT} -d parse_mode=Markdown -d text='${message}'"
                }
            }
        }
    }
        stage('QA Confirmation') {
            steps {
                script {
                    def qaResult = input(
                        message: "Xác nhận kết quả kiểm thử",
                        parameters: [
                            string(name: 'QA_NAME', defaultValue: '', description: 'Tên người kiểm thử'),
                            choice(name: 'TEST_STATUS', choices: 'PASSED\nFAILED', description: 'Kết quả test')
                        ],
                        submitter: "qa"
                    )

                    // Lưu vào biến môi trường để stage sau sử dụng
                    env.APPROVER = qaResult['QA_NAME']

                    if (qaResult['TEST_STATUS'] == 'FAILED') {
                        error "❌ Pipeline bị dừng bởi ${env.APPROVER} do Test thất bại!"
                    }
                    
                    echo "✅ QA ${env.APPROVER} đã phê duyệt bản build này."
                    // --- PHẦN CẬP NHẬT CHANGELOG ---
                    withCredentials([usernamePassword(credentialsId: "${GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')])
                    { 
                        dir('manifest-repo') { 
                            def date = new Date().format('yyyy-MM-dd HH:mm')
                            def newEntry = """
                            ## [Build #${env.BUILD_NUMBER}] - ${date}
                            - **Người duyệt:** ${env.APPROVER}
                            - **Chi tiết thay đổi:**
                            ${env.RELEASE_NOTES}
                            ---
                            """
                            // Đọc nội dung cũ và ghi nội dung mới lên đầu file
                            def changelogFile = readFile('CHANGELOG.md') || ""
                            writeFile(file: 'CHANGELOG.md', text: newEntry + changelogFile)

                            sh """
                                git config user.email "jenkins@bot.com" [cite: 13]
                                git config user.name "Jenkins Bot" [cite: 13]
                                git add CHANGELOG.md
                                git commit -m 'docs: Update CHANGELOG.md for Build ${env.BUILD_NUMBER}' || echo "No changes" 
                                git push https://${GIT_USER}:${GIT_TOKEN}@${GITLAB_REPO_MANIFEST_URL.replace('https://', '')} HEAD:main 
                            """
                        }
                    }
                }
            }
    }

        stage('Push to GCP & GKE Deploy') {
            steps {
                script {
                    // 1. Push lên GCP Artifact Registry
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
                        dir('manifest-repo/ecommerce/overlays/cloud') {
                            sh "kustomize edit set image ecommerce-be=${REGISTRY_URL}/${BE_IMAGE_NAME}:${IMAGE_TAG}"
                            sh "kustomize edit set image ecommerce-fe=${REGISTRY_URL}/${FE_IMAGE_NAME}:${IMAGE_TAG}"
                        }
                    }
                    // 3. Push Git Manifest GKE
                    // 3. Push Git Manifest (SỬ DỤNG LẠI PAT ĐỂ PUSH)
                    withCredentials([usernamePassword(credentialsId: "${GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                        dir('manifest-repo') {
                            sh """
                                git config user.email "jenkins@bot.com"
                                git config user.name "Jenkins Bot"
                                
                                
                                git add ecommerce/overlays/cloud/
                                git commit -m 'GitOps: Deploy to GKE - Build ${IMAGE_TAG}' || echo "No changes to commit"
                                
                                # QUAN TRỌNG: Gán Token vào URL để Push qua HTTPS
                                # URL mẫu: https://user:token@git.codebyluke.io.vn/hybrid-cloud/manifest.git
                                git push https://${GIT_USER}:${GIT_TOKEN}@${GITLAB_REPO_MANIFEST_URL.replace('http://', '').replace('https://', '')} HEAD:main
                            """
                        }
                    }
                }
            }
        }
    }
}