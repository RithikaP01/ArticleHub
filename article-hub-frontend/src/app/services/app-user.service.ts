import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class AppUserService {
  url = environment.apiUrl;

  constructor(private httpClient:HttpClient) { }

  login(data: any): Observable<any>{
    return this.httpClient.post(this.url + "/appuser/login", data, {
      headers: new HttpHeaders().set('content-Type', "application/json")
    })
  }

  addNewAppUser(data: any): Observable<any>{
    return this.httpClient.post(this.url + "/appuser/addNewAppuser", data, {
      headers: new HttpHeaders().set('content-Type', "application/json")
    })
  }

  getAllAppuser(){
    return this.httpClient.get(this.url + "/appuser/getAllAppuser");
  }

  updateUser(data: any): Observable<any>{
    return this.httpClient.post(this.url + "/appuser/updateUser", data, {
      headers: new HttpHeaders().set('content-Type', "application/json")
    })
  }

  updateUserStatus(data: any): Observable<any>{
    return this.httpClient.post(this.url + "/appuser/updateUserStatus", data, {
      headers: new HttpHeaders().set('content-Type', "application/json")
    })
  }
}
