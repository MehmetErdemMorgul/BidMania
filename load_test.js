import http from 'k6/http';
import { sleep, check } from 'k6';

export const options = {
    stages: [
        { duration: '20s', target: 50 },
        { duration: '40s', target: 200 },
        { duration: '20s', target: 0 },
    ],
};

export default function () {
    const url = 'http://localhost:5147/api/Products/';
    const params = {
        headers: {
            'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGF5aV90ZXN0IiwiZXhwIjoxNzc1NDA3MTcwfQ.NH1umSNWX_M3gQfNEX_n_sR28p3yH8sB6FCbf09qzdQ',
            'Content-Type': 'application/json',
        },
    };

    const res = http.get(url, params);

    check(res, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(1);
}