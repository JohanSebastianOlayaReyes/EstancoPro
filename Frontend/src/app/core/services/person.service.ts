import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PersonDto, CreatePersonDto } from '../models/person.model';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private api = inject(ApiService);

  getAll(): Observable<PersonDto[]> {
    return this.api.get<PersonDto[]>('Person');
  }

  getById(id: number): Observable<PersonDto> {
    return this.api.get<PersonDto>(`Person/${id}`);
  }

  create(person: CreatePersonDto): Observable<PersonDto> {
    return this.api.post<PersonDto>('Person', person);
  }

  update(person: PersonDto): Observable<PersonDto> {
    if (!person.id) throw new Error('Person id required');
    return this.api.put<PersonDto>(`Person/${person.id}`, person);
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`Person/logic/${id}`);
  }
}
