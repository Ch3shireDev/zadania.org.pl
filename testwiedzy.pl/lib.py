import requests
import http.cookiejar
import re
import bs4
import sys


def clean(text):
    text = str.join('\n', [line.strip()
                           for line in text.split('\n')]).strip().replace('\n\n', ' ')
    text = re.sub(r' Twoja odpowiedź: \w', '', text)
    return text


def get_answers(id_):
    s = requests.Session()
    i = 0
    cookies = None
    while True:
        headers = {'Content-Type': 'application/x-www-form-urlencoded'}
        payload = f'currentTestQuestion={i}&testToken=1&form_timesum=0&time_stop_value=0&timerValue=0&answer=abc&answer[]=q1'
        i += 1
        response = s.request(
            "POST", f'http://testwiedzy.pl/game/{id_}.html', data=payload, headers=headers, cookies=cookies)
        
        # print(response.text)
        cookies = response.cookies
        m = re.search(r'current-question(?:.+?)>(\d+) / (\d+)',
                      response.text, re.MULTILINE)
        if m:
            a = int(m.group(1))
            b = int(m.group(2))
            print(a,b)
            if i > 1 and a == 1:
                break
        else:
            break

    response = s.request(
        "POST", f'http://testwiedzy.pl/game/{id_}.html', data=payload, headers=headers, cookies=cookies)

    soup = bs4.BeautifulSoup(response.text, features="html.parser")
    answers = soup.find(id='answers')
    try:
        for td in answers.find_all('td'):
            yield [clean(x.text) for x in td.find_all('span')]
    except:
        return None

x = list(get_answers(49421))
print(x)