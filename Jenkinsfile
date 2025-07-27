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
                bat "dotnet restore ${env.PROJECT_PATH}"
            }
        }

        stage('Build') {
            steps {
                bat "dotnet build ${env.PROJECT_PATH} -c ${env.BUILD_CONFIGURATION} --no-restore"
            }
        }

        stage('Unit Tests') {
            steps {
                script {
                    if (fileExists('tests/SAS.IdentityService.Tests.UnitTests/SAS.IdentityService.Tests.UnitTests.csproj')) {
                        bat 'dotnet test tests/SAS.IdentityService.Tests.UnitTests/SAS.IdentityService.Tests.UnitTests.csproj --no-restore --no-build'
                    } else {
                        echo 'No unit test project found — skipping unit tests.'
                    }
                }
            }
        }

        stage('Integration Tests') {
            steps {
                script {
                    if (fileExists('tests/SAS.IdentityService.Tests.IntegrationTests/SAS.IdentityService.Tests.IntegrationTests.csproj')) {
                        // If integration tests require your app running, start Docker Compose first:
                        bat "docker-compose -f ${env.COMPOSE_FILE} up -d --build"

                        // Run integration tests
                        bat 'dotnet test tests/SAS.IdentityService.Tests.IntegrationTests/SAS.IdentityService.Tests.IntegrationTests.csproj --no-restore --no-build'

                        // Tear down containers after tests
                        bat "docker-compose -f ${env.COMPOSE_FILE} down"
                    } else {
                        echo 'No integration test project found — skipping integration tests.'
                    }
                }
            }
        }

        stage('Publish') {
            steps {
                bat "dotnet publish ${env.PROJECT_PATH} -c ${env.BUILD_CONFIGURATION} -o publish /p:UseAppHost=false"
            }
        }

        stage('Build Docker Image') {
            steps {
                bat "docker build -t ${env.IMAGE_NAME} ."
            }
        }

        stage('Run with Docker Compose') {
            steps {
                bat "echo done"
                // bat "docker-compose -f ${env.COMPOSE_FILE} down --remove-orphans"
                // bat "docker-compose -f ${env.COMPOSE_FILE} up -d --build"
            }
        }
    }

    post {
        always {
            echo 'Cleaning up...'
            bat "docker-compose down -v"
        }
    }
}
