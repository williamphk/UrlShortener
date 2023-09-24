# UrlShortener
An implementation of a url shortener using .NET Core.

# CI/CD Pipeline Configuration
This document outlines the Continuous Integration and Continuous Deployment (CI/CD) pipeline configuration and deployment plans for our URL Shortener service. The document aims to guide the development, testing, and deployment processes to ensure seamless integration and rapid delivery of features and bug fixes.

## Tools Used
- Source Control: Git
- CI/CD Platform: Jenkins/CircleCI/GitHub Actions (Choose one)
- Artifact Storage: AWS S3/Google Cloud Storage
- Deployment Platform: Kubernetes
- CI Pipeline Configuration

## CI Pipeline
1. ### Code Check-In
- Trigger: Developer commits to main or feature branches
- Action: Trigger the pipeline
- Tool: Git

2. ### Automated Trigger
- Trigger: Upon code check-in or pull request
- Action: Initialize the pipeline
- Tool: Jenkins/CircleCI/GitHub Actions (Choose one)

3. ### Static Code Analysis
- Trigger: Pipeline Initialization
- Action: Perform linting and static code checks
- Tool: ESLint/SonarQube (Choose one)

4. ### Build
- Trigger: Successful code analysis
- Action: Compile source code and create Docker images
- Tool: Docker, Maven/Gradle (for compilation)

5. ### Unit Testing
- Trigger: Successful build
- Action: Execute unit tests
- Tool: JUnit/xUnit (Choose one)

6. ### Integration Testing
- Trigger: Successful unit tests
- Action: Perform integration tests
- Tool: JUnit/TestNG with Selenium (Choose based on your tech stack)

7. ### Artifact Storage
- Trigger: Successful build and tests
- Action: Store build artifacts in a repository
- Tool: AWS S3/Google Cloud Storage (Choose one)

8. ### Notifications
- Trigger: Any pipeline stage completion
- Action: Notify team of status
- Tool: Email/Slack (Choose one)

## CD Pipeline

1. ### Pull Latest Artifact
- Trigger: Manually or after successful CI pipeline
- Action: Fetch the latest build artifact from storage
- Tool: AWS S3/Google Cloud Storage (Choose one)

2. ### Deployment to Staging
- Trigger: Successful fetch of the latest artifact
- Action: Deploy to staging environment
- Tool: Kubernetes

3. ### Manual Verification
- Trigger: Successful deployment to staging
- Action: QA checks
- Tool: N/A

4. ### Deployment to Production
- Trigger: Successful QA checks
- Action: Deploy to production environment
- Tool: Kubernetes

5. ### Monitoring and Logging
- Trigger: Post-deployment
- Action: Monitor application performance and errors
- Tool: Prometheus/Grafana (Choose one)

# Deployment Plan Document
This document outlines the deployment plan for our web application. It covers the initial deployment strategy, scaling considerations, and update protocols to ensure the efficient and reliable running of the service in production.

## Environment
- Platform: Kubernetes
- Cloud Provider: AWS/Google Cloud/Azure

## Deployment Plan
1. ### Initial Deployment
Dockerized application will be deployed in a Kubernetes cluster.
- Pull the latest Docker image from the artifact storage.
- Configure Kubernetes deployment YAML with the image and necessary environment variables.
- Apply the Kubernetes deployment to roll out the application.
- Validate the deployment by checking pod status and service endpoints.

2. ### Scaling
Automated scaling using Kubernetes Horizontal Pod Autoscaler
- Monitor CPU and Memory usage metrics via Prometheus.
- Set up a Horizontal Pod Autoscaler (HPA) to scale pods based on CPU or Memory.
- Test scaling by simulating increased load and observing the addition or removal of pods.

3. ### Updates
Zero-downtime deployments to ensure service continuity.
- Pull the latest Docker image for the update.
- Update the Kubernetes deployment YAML with the new image.
- Roll out the update using Kubernetes rolling updates.
- Monitor the application and rollback if any issues are detected.

4. ### Monitoring & Logging
Use cloud-native solutions for real-time monitoring and alerting. Prometheus for monitoring and Grafana for visualization. Fluentd and Elasticsearch for logging.
- Set up Prometheus to scrape metrics from the application and Kubernetes nodes.
- Set up Grafana dashboards to visualize these metrics.
- Configure Fluentd to aggregate logs from application pods.

# Real Production Service
While the current solution for URL shortening adheres to the basic requirements set by the task, there are several limitations that could hinder its utility in a real-world production environment. 

## Challenges
### Non-Unique Encryption for Similar URLs
The current encryption method produces identical results for URLs that begin with the same sequence of characters. For example, URLs like https://www.example1.com and https://www.example2.com would yield the same shortened URL (i.e. `ysrAXm`), leading to a conflict.

### Decryption Limitations
The generated short URL contains only the first portion of the full encrypted string, making it impractical to decrypt it back to the original URL. Consequently, users would not be able to be redirected back to the original URL from the shortened one.
 (i.e. `ysrAXmrmzPmkug2ZYBNQgie0lZHIaCAKCPnFCgstjVz6opvkcrw4sdUDO9dUC0tl` vs `ysrAXm`)

### Performance Concerns
Even if we were to include the complete encrypted string in the short URL (i.e. `https://www.example.co/ysrAXmrmzPmkug2ZYBNQgie0lZHIaCAKCPnFCgstjVz6opvkcrw4sdUDO9dUC0tl`), the decryption process could be time-consuming. This approach is not viable as it would result in unacceptable latencies, affecting the user experience adversely.

## Recommendations
### Database-Driven Mapping
For a production-grade solution, it is advisable to adopt a database-driven approach for URL shortening. In this method, a unique random string would be generated for each URL and stored in a database, creating a mapping between the random string and the original URL. This eliminates the need for decryption and allows for faster response times when redirecting users to the original URLs.

### Conflict Avoid
By employing a database-mapping approach, we can ensure that each shortened URL generated is unique. When a new URL is submitted for shortening, the system would generate a random string and check the database to verify that this string has not been used before. If a conflict is detected (i.e., the generated string is already mapped to another URL), the system can regenerate a new random string and perform the verification process again. This ensures that each shortened URL will be unique and correctly mapped to its corresponding original URL, solving the issues related to encryption-based conflicts and non-unique short URLs.
