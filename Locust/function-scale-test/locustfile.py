from locust import HttpLocust, TaskSet

# https://jehollan-scaletestv1nohost.azurewebsites.net/api/Http
def call(l):
    l.client.post("/api/Http", {"scaletest":"01"})

class UserBehavior(TaskSet):
    tasks = {call: 1}

class WebsiteUser(HttpLocust):
    task_set = UserBehavior
    min_wait = 500
    max_wait = 1000