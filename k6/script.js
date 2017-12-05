import http from "k6/http";

export default function() {
    const headers = {
        'Content-Type': 'application/json'
    };

    const payload = {
        "scaletest": "01"
   };
  http.post("https://jehollan-scaletestv1nohost.azurewebsites.net/api/Http", JSON.stringify(payload), {headers});
};