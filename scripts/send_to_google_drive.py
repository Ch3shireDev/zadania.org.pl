# Copyright Hubert & Igor 2020
import pickle
import os.path
import httplib2
from googleapiclient.discovery import build
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request
from apiclient.http import MediaFileUpload

# If modifying these scopes, delete the file token.pickle.
SCOPES = ['https://www.googleapis.com/auth/drive']

"""Shows basic usage of the Drive v3 API.
Prints the names and ids of the first 10 files the user has access to.
"""
creds = None
# The file token.pickle stores the user's access and refresh tokens, and is
# created automatically when the authorization flow completes for the first
# time.
if os.path.exists('token.pickle'):
    with open('token.pickle', 'rb') as token:
        creds = pickle.load(token)
# If there are no (valid) credentials available, let the user log in.
if not creds or not creds.valid:
    if creds and creds.expired and creds.refresh_token:
        creds.refresh(Request())
    else:
        flow = InstalledAppFlow.from_client_secrets_file(
            'credentials.json', SCOPES)
        creds = flow.run_local_server(port=0)
    # Save the credentials for the next run
    with open('token.pickle', 'wb') as token:
        pickle.dump(creds, token)


drive_service = build('drive', 'v2', credentials=creds)

# File body description
media_body = MediaFileUpload("plik.txt", mimetype="text/plain", resumable=True)
body = {
    'title': "plik.txt",
    'description': 'backup',
    'mimeType': "text/plain",
}
# Permissions body description: anyone who has link can upload
# Other permissions can be found at https://developers.google.com/drive/v2/reference/permissions
permissions = {
    'role': 'reader',
    'type': 'anyone',
    'value': None,
    'withLink': True
}
# Insert a file
file = drive_service.files().insert(body=body, media_body=media_body).execute()
# Insert new permissions
drive_service.permissions().insert(fileId=file['id'], body=permissions).execute()
# Define file instance and get url for download
file = drive_service.files().get(fileId=file['id']).execute()
download_url = file.get('webContentLink')

print( 'File ID: %s' % file.get('id'))


# [END drive_quickstart]