// Jenkinsfile - Simple Deploy to Docker Host (Cách 1: All-in-One)
// Builds production images and runs them on the host Docker daemon.
pipeline {
    // Run on the Jenkins controller itself, requires docker-cli installed via Dockerfile
    agent any 

    environment {
        // --- Application & Image Naming ---
        APP_NAME            = 'ecommerce-app' // Base name for images and containers
        USER_DEPLOY         = 'root'
        FRONTEND_IMAGE      = "${APP_NAME}/frontend:latest" // Image name for frontend
        BACKEND_IMAGE       = "${APP_NAME}/backend:latest"  // Image name for backend
        FRONTEND_CONTAINER  = "${APP_NAME}-frontend-app" // Fixed container name for frontend
        BACKEND_CONTAINER   = "${APP_NAME}-backend-app"  // Fixed container name for backend

        // --- Host Port Configuration ---
        // Define ports on the Docker HOST machine where the containers will be accessible.
        // Make sure these ports (e.g., 8081, 5001) are free on your host.
        FRONTEND_HOST_PORT = 8081 // Access Frontend via http://<HOST_IP>:8081
        BACKEND_HOST_PORT  = 5000 // Access Backend via http://<HOST_IP>:5001
        
        // --- Docker Network ---
        // Specify the Docker network the containers should connect to (must exist)
        DOCKER_NETWORK      = 'tonytechlab_default' 


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
                sh "docker compose up ${env.FRONTEND_IMAGE} -d" 
            }
        } // End Stage 2
        
        // --- Stage 4: Deploy Containers to Docker Host ---
        // stage('4. Deploy to Production (Docker Host)') {
        //     steps {
        //         echo "INFO: Approval received. Deploying containers to Docker Host..."
                
        //         // --- Stop and Remove Old Containers ---
        //         // Ensures a clean deployment by removing any previous versions
        //         // '|| true' prevents the pipeline from failing if the container doesn't exist
        //         echo "INFO: Stopping and removing old containers (if they exist)..."
        //         sh "docker stop ${env.FRONTEND_CONTAINER} || true"
        //         sh "docker rm ${env.FRONTEND_CONTAINER} || true"
        //         sh "docker stop ${env.BACKEND_CONTAINER} || true"
        //         sh "docker rm ${env.BACKEND_CONTAINER} || true"

        //         // --- Run New Backend Container ---
        //         echo "INFO: Starting new Backend container..."
        //         // Runs the container using the image built in Stage 2
        //         // -d: Run in detached (background) mode
        //         // --name: Assign a fixed, predictable name
        //         // -p HOST_PORT:CONTAINER_PORT : Map the host port to the container's internal port
        //         //    (Assumes the backend service listens on port 80 inside the container)
        //         // --network: Connect the container to the specified Docker network
        //         // --hostname: Set the hostname inside the container
        //         // --restart always: Ensure the container restarts if it stops or on host reboot
        //         sh "docker run -d --name ${env.BACKEND_CONTAINER} -p ${env.BACKEND_HOST_PORT}:80 --network ${env.DOCKER_NETWORK} --hostname ${env.BACKEND_CONTAINER} --restart always ${env.BACKEND_IMAGE}"
        //         echo "SUCCESS: Backend container started."

        //         // --- Run New Frontend Container ---
        //          echo "INFO: Starting new Frontend container..."
        //         // Assumes the Nginx server inside the frontend image listens on port 80
        //         sh "docker run -d --name ${env.FRONTEND_CONTAINER} -p ${env.FRONTEND_HOST_PORT}:80 --network ${env.DOCKER_NETWORK} --hostname ${env.FRONTEND_CONTAINER} --restart always ${env.FRONTEND_IMAGE}"
        //         echo "SUCCESS: Frontend container started."
                
        //         // --- Output Access Information ---
        //         echo "----------------------------------------------------"
        //         echo "✅ DEPLOYMENT COMPLETE!"
        //         echo "   Access Frontend at: http://<DOCKER_HOST_IP>:${env.FRONTEND_HOST_PORT}"
        //         echo "   Access Backend API at: http://<DOCKER_HOST_IP>:${env.BACKEND_HOST_PORT}"
        //         echo "----------------------------------------------------"
        //         echo "(Replace <DOCKER_HOST_IP> with your host's actual IP, e.g., 192.168.110.161)"
        //     }
        // } // End Stage 4
        // stage('5. Push image to hub'){
        //     steps{
        //         script{
        //              withCredentials([usernamePassword(
        //                         credentialsId: env.REGISTRY_CREDENTIAL, 
        //                         usernameVariable: 'REG_USER', 
        //                         passwordVariable: 'REG_PASS')]
        //                         ){
        //                         sh '''
        //                             echo " Tagging frontend image: ${FRONTEND_IMAGE}"
        //                             docker tag ${FRONTEND_IMAGE} ${REGISTRY_URL}/${USER_DEPLOY}/${FRONTEND_IMAGE}
        //                             echo " Tagging backend image: ${BACKEND_IMAGE}"
        //                             docker tag ${BACKEND_IMAGE} ${REGISTRY_URL}/${USER_DEPLOY}/${BACKEND_IMAGE}
        //                             echo "${REG_PASS}" | docker login ${REGISTRY_URL} -u "${REG_USER}" --password-stdin
        //                             echo " Push images"
        //                             docker push ${REGISTRY_URL}/${USER_DEPLOY}/${BACKEND_IMAGE}
        //                             docker push ${REGISTRY_URL}/${USER_DEPLOY}/${FRONTEND_IMAGE}
        //                             docker logout ${REGISTRY_URL}
        //                         '''
        //         }
        //         }
        //     }
        // }
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