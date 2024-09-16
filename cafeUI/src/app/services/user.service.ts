import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  url = environment.apiUrl;
  constructor(private http: HttpClient) { }

  handleHeaders(): HttpHeaders {
    return new HttpHeaders().set('Content-Type', "application/json");
  }

  signup(data: any) {
    return this.http.post(this.url + "/user/signup", data,
      { headers: this.handleHeaders() })
  }
}
