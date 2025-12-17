module Root_Model

type Model = {
    CustomerAdmin: CustomerAdmin_Model.Model;
    Clock: Clock_Model.Model;
}

type Msg =
    | CustomerAdminMsg of CustomerAdmin_Model.Msg
    | ClockMsg of Clock_Model.Msg
