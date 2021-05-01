export class ObjectStorage implements Storage {
  static instance: ObjectStorage;
  private _keys: any[] = [];
  [key: string]: any;

  private constructor () {
    ObjectStorage.instance = this;
  }

  static getInstance () {
    return ObjectStorage.instance || new ObjectStorage();
  }

  get length (): number {
    return this._keys.length;
  }

  clear (): void {
    this._keys.forEach(key => this.deleteKey(key));
    this._keys = [];
  }

  getItem (key: string): string | null {
    return this[key];
  }

  key (index: number): string | null {
    return this._keys[index];
  }

  removeItem (key: string): void {
    this.deleteKey(key);
    this._keys = this._keys.filter(k => k !== key);
  }

  setItem (key: string, value: string): void {
    this._keys.push(key);
    this[key] = value;
  }

  private deleteKey (key: string): void {
    delete this[key];
  }
}
