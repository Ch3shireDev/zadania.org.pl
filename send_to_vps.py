import sys
import os
import shutil
from datetime import datetime
import pysftp
import paramiko
import glob
from config import server, username, password

try:
    if os.path.exists('build'):
        shutil.rmtree('build')
    os.mkdir('build')
except:
    print("Error removing ./build directory. Aborting...")
    exit()

try:
    os.chdir('ClientApp')
    os.system('ng build --prod')
    os.chdir('..')
    shutil.move('./ClientApp/dist', 'build')
except:
    print('Error compiling Angular project. Aborting...')
    exit()

try:
    if os.path.isdir('./ResourceAPI/ResourceAPI/bin/Release'):
        shutil.rmtree('./ResourceAPI/ResourceAPI/bin/Release')
    os.system('dotnet publish ./ResourceAPI --configuration Release')
    shutil.move('./ResourceAPI/ResourceAPI/bin/Release/netcoreapp3.1/publish', 'build')
except Exception as e:
    print(f'Error compiling ASP.NET project: {e}. Aborting...')
    exit()

try:
    now = datetime.now()
    dt = now.strftime("%Y%m%dT%H%M")
    fname = f'build-{dt}'
    shutil.make_archive(fname, 'zip', 'build')
    fname += '.zip'
    shutil.rmtree('build')
except Exception as e:
    print(f'Error during archving build directory: {e}. Aborting...')
    exit()

try:
    srv = pysftp.Connection(host="zadania.org.pl", username=username,
                            password=password)
    srv.put(fname)
    srv.close()
except Exception as e:
    print(f"Error connecting to SSH: {e}")

client = paramiko.SSHClient()
client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
client.connect(server, username=username, password=password)
client.exec_command('python3 update.py')

for file in glob.glob('build-*.zip'):
    os.remove(file)
