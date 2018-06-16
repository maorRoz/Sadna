from locust import HttpLocust, TaskSet, task

systemId = 0


class WebsiteTasks(TaskSet):

    @task
    def index(self):
        global systemId
        with self.client.get("/api/enter", catch_response=True) as response:
            if response.content == b"":
                response.failure("No data")
            else:
                systemId = response.content.decode("utf-8")
                print(systemId)

    @task
    def add_to_cart(self):
        global systemId
        self.client.get("/shopping/AddToCart?systemId="+systemId+"&state=Guest&store=Toy&product=stress&quantity=1")

    @task
    def buy_from_cart(self):
        global systemId
        additional_string1 = "&state=Guest&store=Toy&product=stress&unitPrice=1&quantity=1&finalPrice=1"
        additional_string2 = "&usernameEntry=Moshe&addressEntry=123&creditCardEntry=12345678"
        self.client.get("/purchase/MakeImmediateBuy?systemId="+systemId+additional_string1+additional_string2)


class WebsiteUser(HttpLocust):
    task_set = WebsiteTasks
    min_wait = 5000
    max_wait = 15000
    host = 'http://localhost:3000'
