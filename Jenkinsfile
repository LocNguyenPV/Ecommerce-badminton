def getLatestTag() {
    // Lấy tag gần nhất, nếu không có tag nào sẽ trả về 0.0.0 hoặc giá trị mặc định
    try {
        def latestTag = sh(script: "git describe --tags --abbrev=0", returnStdout: true).trim()
        return latestTag
    } catch (Exception e) {
        return "0.0.0" // Giá trị mặc định nếu repo chưa có tag nào
    }
}
def validateCommits() {
    def changeLogSets = currentBuild.changeSets
    if (changeLogSets.size() > 0) {
        def codeChangeSet = changeLogSets[0] // Chỉ lấy repo code
        def entries = codeChangeSet.items
        
        // Regex chuẩn Conventional Commits
        def commitPattern = /^(feat|fix|docs|style|refactor|perf|test|chore|revert)(\(.+\))?: .{5,}/

        for (int i = 0; i < entries.length; i++) {
            def entry = entries[i]
            
            // Bỏ qua kiểm tra nếu là commit của Bot
            if (entry.author.fullName.contains("Bot")) continue
            
            if (!(entry.msg =~ commitPattern)) {
                error "❌ Commit message không hợp lệ: '${entry.msg}'\n" +
                      "Vui lòng tuân thủ chuẩn: <type>(<scope>): <subject>\n" +
                      "Ví dụ: feat(api): thêm endpoint lấy danh sách sản phẩm"
            }
        }
    }
}

def calculateSemanticVersion() {
    def currentTag = getLatestTag()
    // Xử lý nếu tag có tiền tố 'v' (ví dụ: v1.2.0)
    def cleanTag = currentTag.startsWith('v') ? currentTag.substring(1) : currentTag
    def (major, minor, patch) = cleanTag.tokenize('.').collect { it.toInteger() }
    // 2. Phân tích các commit trong Repo Code
    def changeLogSets = currentBuild.changeSets
    def isMinor = false
    def isPatch = false

    if (changeLogSets.size() > 0) {
        def entries = changeLogSets[0].items
        for (int i = 0; i < entries.length; i++) {
            def msg = entries[i].msg.toLowerCase()
            if (msg.startsWith("feat")) isMinor = true
            else if (msg.startsWith("fix")) isPatch = true
        }
    }

    // 3. Logic tăng bậc phiên bản
    if (isMinor) {
        minor += 1
        patch = 0
    } else if (isPatch) {
        patch += 1
    } else {
        // Nếu là chore, docs... thì chỉ tăng patch hoặc giữ nguyên
        patch += 1 
    }

    return "${major}.${minor}.${patch}"
}

pipeline {
    agent any
    environment {
        BE_IMAGE_NAME = "ecommerce-be"
        FE_IMAGE_NAME = "ecommerce-fe"

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
                // Checkout code và đảm bảo fetch đủ tags
                checkout([$class: 'GitSCM', 
                    branches: [[name: 'main']], 
                    extensions: [[$class: 'CloneOption', noTags: false, shallow: false]], 
                    userRemoteConfigs: [[url: "${GITLAB_REPO_CODE_URL}", credentialsId: "${GIT_CREDS_ID}"]]
                ])
                script { 
                    // Chạy hàm kiểm tra
                    validateCommits()
                    echo "✅ Tất cả commit message đều hợp lệ!"

                    // Tính toán phiên bản mới
                    def semVer = calculateSemanticVersion()
                    def buildNum = env.BUILD_NUMBER

                    env.ON_PREM_TAG = "${semVer}-build.${buildNum}" // Dùng cho Harbor: 1.2.0-build.45
                    env.CLOUD_TAG   = "${semVer}"             // Dùng cho GKE: 1.2.0
                    echo "🚀 Version: On-Premise (${env.ON_PREM_TAG}) | Cloud (${env.CLOUD_TAG})"
                    // Bọc parallel để build song song
                    parallel(
                        "Build Backend": {
                            dir('ECommerce.ProductManagement') { sh "docker build -t ${BE_IMAGE_NAME}:${BUILD_NUMBER} ." }
                        },
                        "Build Frontend": {
                            dir('ecommerce-badminton-fe') { sh "docker build -t ${FE_IMAGE_NAME}:${BUILD_NUMBER} ." }
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
                        sh "docker tag ${BE_IMAGE_NAME}:${BUILD_NUMBER} ${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${ON_PREM_TAG}"
                        sh "docker tag ${FE_IMAGE_NAME}:${BUILD_NUMBER} ${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${ON_PREM_TAG}"
                        sh "docker push ${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${ON_PREM_TAG}"
                        sh "docker push ${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${ON_PREM_TAG}"
                    }
                    
                    // 2. Kustomize cho on-premise
                    docker.image('line/kubectl-kustomize').inside {
                        dir('manifest-repo/ecommerce/overlays/on-premise') {
                            sh "kustomize edit set image ecommerce-be=${HARBOR_HOST}/${HARBOR_PROJECT}/${BE_IMAGE_NAME}:${ON_PREM_TAG}"
                            sh "kustomize edit set image ecommerce-fe=${HARBOR_HOST}/${HARBOR_PROJECT}/${FE_IMAGE_NAME}:${ON_PREM_TAG}"
                        }
                    }

                    // 3. Push Git Manifest (SỬ DỤNG LẠI PAT ĐỂ PUSH)
                    withCredentials([usernamePassword(credentialsId: "${GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                        dir('manifest-repo') {
                            sh """
                                git config user.email "jenkins@bot.com"
                                git config user.name "Jenkins Bot"
                                
                                
                                git add ecommerce/overlays/on-premise/
                                git commit -m 'GitOps: Deploy to On-premise - Build ${ON_PREM_TAG}' || echo "No changes to commit"
                                
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
                    def beChanges = []
                    def feChanges = []
                    def otherChanges = []
                    // Lấy danh sách thay đổi của lần checkout đầu tiên (Repo Code)
                    def changeLogSets = currentBuild.changeSets
                    
                    if (changeLogSets.size() > 0) {
                        // Chỉ xử lý phần tử đầu tiên - tương ứng với repository code 
                        def codeChangeSet = changeLogSets[0]
                        def entries = codeChangeSet.items
                        
                        for (int j = 0; j < entries.length; j++) {
                            def entry = entries[j]
                            
                            // Bỏ qua nếu là commit của Bot (nếu có trong repo code)
                            if (entry.author.fullName.contains("Bot")) continue
                            
                            def commitMsg = "- ${entry.msg} (${entry.author.fullName})"
                            def files = entry.affectedFiles
                            
                            // Phân loại dựa trên cấu trúc thư mục của bạn 
                            if (files.any { it.path.contains('ECommerce.ProductManagement') }) {
                                beChanges.add(commitMsg)
                            } else if (files.any { it.path.contains('ecommerce-badminton-fe') }) {
                                feChanges.add(commitMsg)
                            }else{
                                otherChanges.add(commitMsg)
                            }
                        }
                    }

                    // Tổng hợp nội dung Release Notes
                    def finalNotes = ""
                    if (beChanges) finalNotes += "\n*BACKEND:*\n" + beChanges.join("\n")
                    if (feChanges) finalNotes += "\n*FRONTEND:*\n" + feChanges.join("\n")
                    if (otherChanges) finalNotes += "\n*OTHER:*\n" + otherChanges.join("\n")
                    
                    env.RELEASE_NOTES = finalNotes ?: "- Không có thay đổi mã nguồn trong phiên build này."
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
                        
                        📝 *Cập nhật mới (Release Notes):\n*
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
                                git config user.email "jenkins@bot.com" 
                                git config user.name "Jenkins Bot"
                                git add CHANGELOG.md
                                git commit -m 'docs: Update CHANGELOG.md for TAG ${CLOUD_TAG}' || echo "No changes" 
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
                            
                            def beGCP = "${REGISTRY_URL}/${BE_IMAGE_NAME}:${CLOUD_TAG}"
                            def feGCP = "${REGISTRY_URL}/${FE_IMAGE_NAME}:${CLOUD_TAG}"

                            sh "docker tag ${BE_IMAGE_NAME}:${BUILD_NUMBER} ${beGCP}"
                            sh "docker tag ${FE_IMAGE_NAME}:${BUILD_NUMBER} ${feGCP}"
                            sh "docker push ${beGCP}"
                            sh "docker push ${feGCP}"
                    }

                    
                    // 2. Kustomize cho GKE
                    docker.image('line/kubectl-kustomize').inside {
                        dir('manifest-repo/ecommerce/overlays/cloud') {
                            sh "kustomize edit set image ecommerce-be=${REGISTRY_URL}/${BE_IMAGE_NAME}:${CLOUD_TAG}"
                            sh "kustomize edit set image ecommerce-fe=${REGISTRY_URL}/${FE_IMAGE_NAME}:${CLOUD_TAG}"
                        }
                    }

                    // 3. Push Git Manifest (SỬ DỤNG LẠI PAT ĐỂ PUSH)
                    withCredentials([usernamePassword(credentialsId: "${GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                        // 1. Tag Repo CODE (Quan trọng nhất để tăng version cho lần sau)
                        dir('code-repo') { 
                            sh """
                                git tag -a v${CLOUD_TAG} -m "Release v${CLOUD_TAG} approved by ${env.APPROVER}"
                                git push https://${GIT_USER}:${GIT_TOKEN}@${GITLAB_REPO_CODE_URL.replace('https://', '')} --tags
                            """
                        }
                        dir('manifest-repo') {
                            sh """
                                git config user.email "jenkins@bot.com"
                                git config user.name "Jenkins Bot"
                                
                                
                                git add ecommerce/overlays/cloud/
                                git commit -m 'GitOps: Deploy to GKE - Build ${CLOUD_TAG}' || echo "No changes to commit"
                                git tag -a v${CLOUD_TAG} -m "Release ${CLOUD_TAG}"

                                # QUAN TRỌNG: Gán Token vào URL để Push qua HTTPS
                                # URL mẫu: https://user:token@git.codebyluke.io.vn/hybrid-cloud/manifest.git
                                git push https://${GIT_USER}:${GIT_TOKEN}@${GITLAB_REPO_MANIFEST_URL.replace('http://', '').replace('https://', '')} HEAD:main
                            """
                        }
                    }
                }
            }
        }

        stage('Git Tagging') {
            steps {
                script {
                    // Sử dụng lại thông tin từ Jenkinsfile của bạn
                    withCredentials([usernamePassword(credentialsId: "${GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) { 
                        dir('manifest-repo') {
                            def tagName = "v${CLOUD_TAG}" // Bạn có thể tùy biến format tag 
                            def tagMessage = "Release Build ${CLOUD_TAG} - Approved by ${env.APPROVER}"

                            sh """
                                git config user.email "jenkins@bot.com"
                                git config user.name "Jenkins Bot"
                                
                                # Tạo tag cục bộ
                                git tag -a ${tagName} -m "${tagMessage}"
                                
                                #Push tag lên GitLab sử dụng PAT
                                git push https://${GIT_USER}:${GIT_TOKEN}@${GITLAB_REPO_MANIFEST_URL.replace('https://', '')} ${tagName}
                            """
                            echo "🚀 Đã tạo và push tag ${tagName} lên GitLab!"
                        }
                    }
                }
            }
        }
    }
}