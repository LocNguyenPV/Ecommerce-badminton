def label = "dind-${UUID.randomUUID().toString()}"

podTemplate(label: label, yaml: """
apiVersion: v1
kind: Pod
metadata:
  labels:
    jenkins: agent
spec:
  containers:
  - name: docker     # Container chứa lệnh CLI để bạn gõ
    image: docker:20.10.21
    command: ['cat']
    tty: true
    env:
    - name: DOCKER_HOST
      value: tcp://localhost:2375  # Trỏ vào thằng DinD bên dưới
  - name: dind       # Container chạy Docker Daemon (Docker ảo)
    image: docker:20.10.21-dind
    securityContext:
      privileged: true # BẮT BUỘC: Để được quyền tạo Docker con
    env:
    - name: DOCKER_TLS_CERTDIR
      value: ""      # Tắt TLS cho dễ kết nối nội bộ
"""
) {
    node(label) {
        // --- CẤU HÌNH ---
        // QUAN TRỌNG: Dùng DNS nội bộ K8s để gọi Harbor (Vì DinD không hiểu harbor.local)
        // Nếu namespace của Harbor là 'devops-tools', service thường là:
        env.HARBOR_HOST = 'harbor-harbor-registry.devops-tools.svc.cluster.local:80'
        
        env.HARBOR_PROJECT = 'ecommerce-badminton'
        env.GITLAB_REPO_URL = 'https://gitlab.codebyluke.io.vn/root/ecommerce-badminton-hub.git'
        
        env.HARBOR_CREDS_ID = 'harbor-credentials'
        env.GIT_CREDS_ID = 'gitlab-credentials'

        stage('Checkout Code') {
            git branch: 'main', 
                credentialsId: "${env.GIT_CREDS_ID}", 
                url: "${env.GITLAB_REPO_URL}"
        }

        stage('Wait for Docker') {
            container('docker') {
                // Chờ một chút cho DinD khởi động
                sh 'sleep 5'
                sh 'docker version' // Test thử xem có Docker chưa
            }
        }

        stage('Build & Push Backend') {
            container('docker') {
                script {
                    echo '--- BUILDING BACKEND ---'
                    def beImage = "${env.HARBOR_HOST}/${env.HARBOR_PROJECT}/ecommerce-be:${env.BUILD_NUMBER}"
                    
                    dir('ECommerce.ProductManagement') {
                        sh "docker build -t ${beImage} ."
                    }
                    
                    // Login vào Harbor
                    withCredentials([usernamePassword(credentialsId: "${env.HARBOR_CREDS_ID}", usernameVariable: 'USER', passwordVariable: 'PASS')]) {
                        // Vì Harbor nội bộ thường không có SSL xịn, ta thêm --insecure-registry nếu cần
                        // Nhưng docker login vẫn cần user/pass
                        sh "docker login ${env.HARBOR_HOST} -u $USER -p $PASS"
                        sh "docker push ${beImage}"
                    }
                }
            }
        }

        stage('Build & Push Frontend') {
            container('docker') {
                script {
                    echo '--- BUILDING FRONTEND ---'
                    def feImage = "${env.HARBOR_HOST}/${env.HARBOR_PROJECT}/ecommerce-fe:${env.BUILD_NUMBER}"
                    
                    dir('ecommerce-badminton-fe') {
                        sh "docker build -t ${feImage} ."
                    }
                    
                    sh "docker push ${feImage}"
                }
            }
        }

        stage('Update GitOps') {
            // Bước này chạy ở container jnlp (mặc định)
            script {
                sh 'git config user.email "jenkins@bot.com"'
                sh 'git config user.name "Jenkins Bot"'
                
                // Update Image Tag trong file K8s
                sh "sed -i 's|ecommerce-be:.*|ecommerce-be:${env.BUILD_NUMBER}|g' k8s/03-backend.yaml"
                sh "sed -i 's|ecommerce-fe:.*|ecommerce-fe:${env.BUILD_NUMBER}|g' k8s/04-frontend.yaml"
                
                // QUAN TRỌNG: Sửa luôn domain Harbor trong file yaml thành Internal DNS
                // Để lúc Pod chạy thật trên K8s nó pull được ảnh
                def internalHarbor = env.HARBOR_HOST
                sh "sed -i 's|harbor.local:30080|${internalHarbor}|g' k8s/03-backend.yaml"
                sh "sed -i 's|harbor.local:30080|${internalHarbor}|g' k8s/04-frontend.yaml"

                withCredentials([usernamePassword(credentialsId: "${env.GIT_CREDS_ID}", usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                    def repoClean = env.GITLAB_REPO_URL.replace("http://", "")
                    sh "git add k8s/"
                    sh "git commit -m 'Jenkins Update Image to Build ${env.BUILD_NUMBER}' || true" 
                    sh "git push http://${GIT_USER}:${GIT_PASS}@${repoClean} HEAD:main"
                }
            }
        }
    }
}