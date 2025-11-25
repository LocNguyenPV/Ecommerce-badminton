// Jenkinsfile - Simple Deploy to Docker Host (Cách 1: All-in-One)
// Builds production images and runs them on the host Docker daemon.
pipeline {
    // Run on the Jenkins controller itself, requires docker-cli installed via Dockerfile
    agent any 

    environment {
        // --- Application & Image Naming ---
        APP_NAME            = 'ecommerce' // Base name for images and containers
        USER_DEPLOY         = 'root'
        FRONTEND_IMAGE      = "badminton-store-fe:latest" // Image name for frontend
        BACKEND_IMAGE       = "badminton-store-be:latest"  // Image name for backend
        FRONTEND_CONTAINER  = "${APP_NAME}_fe" // Fixed container name for frontend
        BACKEND_CONTAINER   = "${APP_NAME}_be"  // Fixed container name for backend

        // --- Host Port Configuration ---
        // Define ports on the Docker HOST machine where the containers will be accessible.
        // Make sure these ports (e.g., 8081, 5001) are free on your host.
        FRONTEND_HOST_PORT = 8081 // Access Frontend via http://<HOST_IP>:8081
        BACKEND_HOST_PORT  = 5000 // Access Backend via http://<HOST_IP>:5001
        
        // --- Docker Network ---
        // Specify the Docker network the containers should connect to (must exist)
        DOCKER_NETWORK      = '' 


        // --- Registry Config --
        REGISTRY_URL = 'registry.codebyluke.io.vn'       
        REGISTRY_CREDENTIAL = 'docker-registry-creds'       
    }

    stages {
        // --- Stage 1: Get latest code ---
        stage('1. Checkout Code') {
            steps {
                // Use Jenkins built-in SCM checkout step
                checkout scm 
                echo "SUCCESS: Code checked out from GitLab."
            }
        }
        
        // --- Stage 2: Build Production Docker Images ---
        stage('2. Run Docker Compose') {
            steps {
                echo "INFO: Building Docker Compose"
                sh "docker compose down -v" 
                sh "docker stop ${env.FRONTEND_CONTAINER} || true"
                sh "docker rm ${env.FRONTEND_CONTAINER} || true"
                sh "docker rmi ${env.FRONTEND_IMAGE} || true"
                sh "docker stop ${env.BACKEND_CONTAINER} || true"
                sh "docker rm ${env.BACKEND_CONTAINER} || true"
                sh "docker rmi ${env.BACKEND_IMAGE} || true"
                sh "docker compose up -d" 
            }
        } // End Stage 2
        
    } // End of stages

    // --- Post-build Actions ---
    // Actions to perform after the pipeline finishes
    post { 
        always { // Always run these steps
            echo 'INFO: Pipeline finished execution.'
            // cleanWs() // Option to clean the Jenkins workspace
        }
        success { // Run only on success
            echo '✅ SUCCESS: Pipeline completed successfully!'
            // Add success notifications (email, Slack, etc.) here
        }
        failure { // Run only on failure
            echo '❌ FAILED: Pipeline failed!'
            // Add failure notifications here
        }
    } // End of post
    
} // End of pipeline