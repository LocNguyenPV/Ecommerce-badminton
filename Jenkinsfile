pipeline {
    // THAY ĐỔI QUAN TRỌNG: KHÔNG DÙNG 'agent any' NỮA
    agent any
    environment {
                // --- CẤU HÌNH ---
        // QUAN TRỌNG: Dùng DNS nội bộ K8s để gọi Harbor (Vì DinD không hiểu harbor.local)
        // Nếu namespace của Harbor là 'devops-tools', service thường là:
        HARBOR_HOST = 'harbor-harbor-registry.devops-tools.svc.cluster.local:80'
        
        HARBOR_PROJECT = 'ecommerce-badminton'
        GITLAB_REPO_URL = 'https://gitlab.codebyluke.io.vn/root/ecommerce-badminton-hub.git'
        
        HARBOR_CREDS_ID = 'harbor-creds'
        GIT_CREDS_ID = 'gitlab-token'
    }
    
    stages {
        stage('Checkout Code') {
            steps {
                // Lấy code mới nhất về
                git branch: 'main', 
                    credentialsId: "${env.GIT_CREDS_ID}", 
                    url: "${env.GITLAB_REPO_URL}"
            }
        }
        
        stage('Build & Push Backend') {
            steps {
                script {
                    echo '--- BUILDING BACKEND ---'
                    // Định nghĩa tên ảnh với Tag là số lần build (Build Number)
                    def beImage = "${HARBOR_HOST}/${HARBOR_PROJECT}/ecommerce-be:${env.BUILD_NUMBER}"
                    
                    // Build Docker (Dùng Dockerfile trong thư mục BE)
                    dir('ECommerce.ProductManagement') {
                        sh "docker build -t ${beImage} ."
                    }
                    
                    // Đăng nhập và Push lên Harbor
                    withCredentials([usernamePassword(credentialsId: "${env.HARBOR_CREDS_ID}", usernameVariable: 'USER', passwordVariable: 'PASS')]) {
                        sh "docker login ${HARBOR_HOST} -u $USER -p $PASS"
                        sh "docker push ${beImage}"
                    }
                }
            }
        }
        
        stage('Build & Push Frontend') {
            steps {
                script {
                    echo '--- BUILDING FRONTEND ---'
                    def feImage = "${HARBOR_HOST}/${HARBOR_PROJECT}/ecommerce-fe:${env.BUILD_NUMBER}"
                    
                    // Build Docker (Dùng Dockerfile trong thư mục FE)
                    dir('ecommerce-badminton-fe') {
                        // Lưu ý: NextJS cần biến môi trường lúc Build nếu dùng static generation
                        // Ở đây ta build image bình thường
                        sh "docker build -t ${feImage} ."
                    }
                    
                    // Push
                    sh "docker push ${feImage}"
                }
            }
        }
        
        stage('Update Manifest (GitOps)') {
            steps {
                script {
                    echo '--- UPDATING K8S MANIFESTS ---'
                    
                    // Cấu hình Git để commit
                    sh 'git config user.email "jenkins@bot.com"'
                    sh 'git config user.name "Jenkins Bot"'
                    
                    // 1. Sửa file Backend (Thay tag cũ bằng tag BUILD_NUMBER mới)
                    // Tìm dòng chứa "image: .../ecommerce-be:" và thay số đuôi
                    sh "sed -i 's|ecommerce-be:.*|ecommerce-be:${env.BUILD_NUMBER}|g' k8s/03-backend.yaml"
                    
                    // 2. Sửa file Frontend
                    sh "sed -i 's|ecommerce-fe:.*|ecommerce-fe:${env.BUILD_NUMBER}|g' k8s/04-frontend.yaml"
                    
                    // 3. Commit và Push ngược lại GitLab
                    // ArgoCD sẽ thấy thay đổi này và tự deploy
                    withCredentials([usernamePassword(credentialsId: "${env.GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        // Cần set lại URL có chứa user:pass để push được
                        // Cắt bỏ http:// đầu để chèn user:pass vào
                        def repoClean = env.GITLAB_REPO_URL.replace("http://", "")
                        sh "git add k8s/"
                        sh "git commit -m 'Jenkins Update Image to Build ${env.BUILD_NUMBER}'"
                        sh "git push http://${GIT_USER}:${GIT_PASS}@${repoClean} HEAD:main"
                    }
                }
            }
        }
    }
}