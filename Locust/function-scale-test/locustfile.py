from locust import HttpLocust, TaskSet, task
import resource
print(resource.getrlimit(resource.RLIMIT_STACK))

# https://jehollan-scaletestv1nohost.azurewebsites.net/api/Http
class MyTaskSet(TaskSet):
    @task
    def call(l):
        r = l.client.post("/api/Http", {"scaletest":"01"})
        r.raise_for_status()

class MyLocust(HttpLocust):
    task_set = MyTaskSet
    min_wait = 0
    max_wait = 0