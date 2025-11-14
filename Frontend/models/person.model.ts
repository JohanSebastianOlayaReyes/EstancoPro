export interface PersonDto {
  id?: number;
  fullName: string;
  firstName?: string;
  firstLastName?: string;
  phoneNumber?: number;
  numberIdentification?: number;
}

export interface CreatePersonDto {
  fullName: string;
  firstName?: string;
  firstLastName?: string;
  phoneNumber?: number;
  numberIdentification?: number;
}
