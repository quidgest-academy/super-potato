# PowerShell Script: Post Full Server Status Dashboard
 
# Define variables (replace with your actual values)
$grafanaApiUrl = "http://localhost:3001"  # Replace with your Grafana URL
$adminUser = "quidgest"
$adminPassword = "zph2lab"

# Encode credentials for Basic Auth
$bytes = [System.Text.Encoding]::UTF8.GetBytes("$adminUser`:$adminPassword")
$base64Auth = [Convert]::ToBase64String($bytes)
 
# Set headers (basic auth) 
$headers = @{
    "Content-Type"  = "application/json"
    "Authorization" = "Basic $base64Auth"
}

# Set headers (service account)
#$headers = @{
#    "Content-Type"  = "application/json"
#    "Authorization" = "Bearer $adminPassword"
#}

# Fix url, just to make sure
$grafanaApiUrl = $grafanaApiUrl.TrimEnd('/')

#-----------------------------------
# Create datasource
#-----------------------------------

# Send POST request to Grafana API
$payload = Get-Content -Raw -Path "datasource_prometheus.json"
Invoke-RestMethod -Uri "$grafanaApiUrl/api/datasources" `
                  -Method POST `
                  -Headers $headers `
                  -Body $payload `
                  -UseBasicParsing `
                  -SkipCertificateCheck  # only use this if you're connecting to a self-signed instance

# Send POST request to Grafana API
$payload = Get-Content -Raw -Path "datasource_loki.json"
Invoke-RestMethod -Uri "$grafanaApiUrl/api/datasources" `
                  -Method POST `
                  -Headers $headers `
                  -Body $payload `
                  -UseBasicParsing `
                  -SkipCertificateCheck  # only use this if you're connecting to a self-signed instance


#-----------------------------------
# Create dashboard
#-----------------------------------

# Send POST request to Grafana API
$payload = Get-Content -Raw -Path "dashboard_metrics.json"
Invoke-RestMethod -Uri "$grafanaApiUrl/api/dashboards/db" `
                  -Method POST `
                  -Headers $headers `
                  -Body $payload `
                  -UseBasicParsing `
                  -SkipCertificateCheck  # only use this if you're connecting to a self-signed instance
				  
# Send POST request to Grafana API
$payload = Get-Content -Raw -Path "dashboard_logs.json"
Invoke-RestMethod -Uri "$grafanaApiUrl/api/dashboards/db" `
                  -Method POST `
                  -Headers $headers `
                  -Body $payload `
                  -UseBasicParsing `
                  -SkipCertificateCheck  # only use this if you're connecting to a self-signed instance
