pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        BUILD_CONFIGURATION = 'Release'
        PROJECT_PATH = 'src/SAS.IdentityService.API/SAS.IdentityService.API.csproj'
        IMAGE_NAME = 'sas.identityservice'
        COMPOSE_FILE = 'docker-compose.yml'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore Dependencies') {
            steps {
                sh "dotnet restore ${env.PROJECT_PATH}"
            }
        }

        stage('Build') {
            steps {
                sh "dotnet build ${env.PROJECT_PATH} -c ${env.BUILD_CONFIGURATION} --no-restore"
            }
        }

        stage('Test') {
            steps {
                // Optional: run if test project exists
                script {
                    if (fileExists('src/SAS.IdentityService.Tests/SAS.IdentityService.Tests.csproj')) {
                        sh "dotnet test src/SAS.IdentityService.Tests/SAS.IdentityService.Tests.csproj --no-restore --no-build"
                    } else {
                        echo 'No test project found — skipping tests.'
                    }
                }
            }
        }

        stage('Publish') {
            steps {
                sh "dotnet publish ${env.PROJECT_PATH} -c ${env.BUILD_CONFIGURATION} -o publish /p:UseAppHost=false"
            }
        }

        stage('Build Docker Image') {
            steps {
                sh "docker build -t ${env.IMAGE_NAME} ."
            }
        }

        stage('Run with Docker Compose') {
            steps {
                sh "docker-compose -f ${env.COMPOSE_FILE} up -d --build"
            }
        }
    }

    post {
        always {
            echo 'Cleaning up...'
            sh "docker-compose down -v"
        }
    }
}
