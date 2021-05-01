import { DataCreationErrors, GeneralApiErrors, HttpStatusCode } from '@shared/enums';

export class ApiError extends Error {
  status: number;
  type: string = '';
  // body: any; // TODO: Possible will be needed

  constructor (errorInstance: any) {
    super(errorInstance);

    this.status = Number(errorInstance.status);
    this.type = errorInstance.type || '';
  }

  static dataExistsParser (e: ApiError) {
    return new ApiError({
      ...e,
      type: e.status === HttpStatusCode.CONFLICT
        ? DataCreationErrors.ALREADY_EXIST
        : GeneralApiErrors.DEFAULT,
    });
  }
}
