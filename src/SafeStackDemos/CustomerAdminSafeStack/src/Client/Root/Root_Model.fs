module Root_Model

type Model = {
    Clock: Clock_Model.Model;
    CustomerAdmin: CustomerAdmin_Model.Model;
}

type Msg =
    | ClockMsg of Clock_Model.Msg
    | CustomerAdminMsg of CustomerAdmin_Model.Msg
