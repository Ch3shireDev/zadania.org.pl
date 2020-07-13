from lib import get_answers, clean
from bs4 import BeautifulSoup
import re
import requests

def get_ids(href):
    req = requests.request('get', f'http://testwiedzy.pl{href}')
    soup = BeautifulSoup(req.text, features='html.parser')
    blocklist = soup.find(id='blocklist')
    for td in blocklist.find_all('td', class_='img'):
        for a in td.find_all('a'):
            href = a.attrs['href']
            m = re.match(r'/test/(\d+)/.*', href)
            if not m:
                continue
            answers, path =  get_test(href, m.group(1))
            yield answers,path


def get_categories(href):
    req = requests.request('get', f'http://testwiedzy.pl{href}')
    soup = BeautifulSoup(req.text, features='html.parser')
    tag = soup.find('p', class_="pagination")
    num = tag.text.split()[-3].strip()
    num = int(num)
    href = href.replace('.html', '')
    for i in range(num):
        data = f"{href}/{i+1}.html"
        for answers, path in get_ids(data):
            yield answers, path
        

def get_test(href, num):
    req = requests.request('get', f'http://testwiedzy.pl{href}')
    soup = BeautifulSoup(req.text, features='html.parser')
    x = soup.find(id="breadcrumb")
    text = clean(x.text)
    answers = list(get_answers(f'http://testwiedzy.pl/game/{num}.html'))
    path = [line.strip() for line in text.split('»')]
    return answers, path
    
categories = [
'/categories/28/testy-specjalne-z-roznych-egzaminow-szkolnych.html',
'/categories/11/testy-z-ekonomii.html',
'/categories/7/testy-z-informatyki.html',
'/categories/1/testy-z-nauki.html'
]
    
req = requests.request('get', 'http://testwiedzy.pl')
soup = BeautifulSoup(req.text, features="html.parser")
for href in categories:
    elements = get_categories(href)
    i = 0
    for answers, path in elements:
        i+=1
        if len(answers) == 0:
            print(i, path)